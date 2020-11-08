# CheckTLS
Queste utilities aiutanto a verificare se il sistema su cui è installato Mago4 o Mago.net (client o server) sia predisposto per l'utilizzo del protocollo di sicurezza TLS 1.2.

Tale protocollo è quello utilizzato dal Digital Hub a partire dal 30 Novembre 2020, per cui non sarà possibile operare con la Fatturazione Elettronica se tale protocollo non viene supportato.

Le utilities sono qui presenti in formato sorgente, sotto forma di solution per Visual Studio 2019. È possibile scaricare le utilities in formato eseguibile dall'apposita pagina dell'Help Center.

## Struttura
Ci sono due versioni delle utilities, una per Mago.net e una per Mago4: questo è dovuto al diverso .NET Framework utilizzato dai due prodotti (4.0 per Mago.net e 4.6.1 per Mago4)

Il tool prova inizialmente un collegamento con Digital Hub con impostazioni di default. Se il collegamento riesce, questo significa che il sistema sta già utilizzando il TLS 1.2 e quindi Mago può già operare correttamente.

Se questo primo tentativo non va a buon fine il tool prova un collegamento forzando per via programmativa l'utilizzo del TLS 1.2. Se questo ha successo, significa che il sistema può essere utilizzato, ma è necessario installare gli ultimi aggiornamenti di Mago.  
In alternativa si può configurare il sistema tramite il registry per forzare l'uso di tale protocollo, si vedano le apposite pagine dell'Help Center per dettagli.

Se anche questo secondo tentativo fallisce, il sistema non è in grado di utilizzare il protocollo TLS 1.2.  
Sarà necessario prima installare tutti gli aggiornamenti disponibili, o passare ad una versione di S.O. più moderna.
