using Microsoft.Win32;
using System.Collections.Generic;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Microarea.CheckTLS.MagoNet
{
    class Program
    {
        static void printRegistryValue(string keyName, string valueName)
        {
            var regkey = Registry.GetValue(keyName, valueName, null);
            Console.WriteLine($"{keyName}:{valueName}={(regkey != null ? regkey : "[non impostato]") }");
        }

        static void printRegistryValues()
        {
            Console.WriteLine("\nChiavi di registro relative a TLS:");
            Console.WriteLine("\n'SchUseStrongCrypto'");
            printRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\.NETFramework\v4.0.30319", "SystemDefaultTlsVersions");
            printRegistryValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\.NETFramework\v4.0.30319", "SchUseStrongCrypto");
            Console.WriteLine("\n'SCHANNEL-Protocols-TLS'");
            printRegistryValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\SecurityProviders\SCHANNEL\Protocols\TLS 1.2\Client", "DisabledByDefault");
            printRegistryValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\SecurityProviders\SCHANNEL\Protocols\TLS 1.2\Client", "Enabled");
            printRegistryValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\SecurityProviders\SCHANNEL\Protocols\TLS 1.2\Server", "DisabledByDefault");
            printRegistryValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\SecurityProviders\SCHANNEL\Protocols\TLS 1.2\Server", "Enabled");
            Console.WriteLine("");
        }

        static void Main(string[] args)
        {
            //string DHURL = "https://fatturapa-test.zucchetti.it/test/services/fatelwV1"
            string DHURL = "https://digitalhubtest.zucchetti.it/fatelw/services/fatelwV1";

            Console.WriteLine("=====================");
            Console.WriteLine("CheckTLS - Mago.net");
            Console.WriteLine("=====================");
            Console.WriteLine($"v. {Assembly.GetExecutingAssembly().GetName().Version}");

            Console.Write("\nverifica in corso ...\r");
            
            // inizialmente si prova senza forzare l'uso del protocollo (precedenti versioni di Mago)
            DigitalHub_Wrapper digitalHub_Wrapper = new DigitalHub_Wrapper(DHURL, false);

            // la connessione non andrà mai a buon fine, le credenziali sono fittizie
            digitalHub_Wrapper.Connect("test", "test", "test");

            // il fatto che la connessione sia disponibile viene dedotto dal tipo di errore
            var error = digitalHub_Wrapper.GetWSError();

            // se viene dato errore di credenziali significa che il WS è stato raggiunto con il protocollo TLS 1.2
            if (
                    error.StartsWith("Wrong username or password") ||
                    error.StartsWith("The HTTP request was forbidden with client authentication scheme 'Anonymous'") ||
                    error.StartsWith("Richiesta HTTP vietata con lo schema di autenticazione client 'Anonymous'")
               )
            {
                Console.WriteLine("Connessione con Digital Hub riuscita\nIl sistema supporta correttamente il protocollo TLS 1.2.");
            }
            // se viene dato errore per mancanza del protocollo TLS 1.2, bisogna aggiornare od intervenire sul sistema
            else if (
                        error.StartsWith("Could not establish secure channel for SSL/TLS with authority") ||
                        error.StartsWith("Impossibile stabilire un canale sicuro per SSL/TLS con")
                    )
            {
                // si prova con la forzatura dell'uso del protocollo (ultime versioni di Mago)
                digitalHub_Wrapper = new DigitalHub_Wrapper(DHURL, true);

                digitalHub_Wrapper.Connect("test", "test", "test");
                error = digitalHub_Wrapper.GetWSError();
                if (
                        error.StartsWith("Wrong username or password") ||
                        error.StartsWith("The HTTP request was forbidden with client authentication scheme 'Anonymous'") ||
                        error.StartsWith("Richiesta HTTP vietata con lo schema di autenticazione client 'Anonymous'")
                   )
                {
                    Console.WriteLine("Connessione con Digital Hub possibile\nIl sistema supporta il protocollo TLS 1.2., ma è necessario aggiornare Mago oppure applicare opportune configurazioni al registry.");
                    printRegistryValues();
                }
                else
                {
                    Console.WriteLine($"Connessione con Digital Hub **FALLITA**\nIl sistema **NON** supporta il protocollo TLS 1.2, è necessario intervenire.\n({error})");
                    printRegistryValues();
                }
            }
            // altri errori sono problemi di sistema di altro tipo (es.: mancanza di connettività, ecc.)
            else
            {
                Console.WriteLine($"Errore sconosciuto:\n{error}");
                printRegistryValues();
            }

            Console.WriteLine("\n[premere un tasto per chiudere]");
            Console.ReadKey();

        }
    }
}
