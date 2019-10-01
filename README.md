# webwatchfn
An Azure Function to monitor HTML pages for updates and notify a user with a diff of the changes.

To deploy to Azure:
1. Create Azure Service principle
2. Populate ./Deploy/config
  All parameters are required including:
    * subscription - Azure subscription ID
    * location - Azure region for the deployment (e.g. centralus)
    * appID - App ID for Service Principle
    * password - Password for Service Principle
    * tenant - Azure tenant ID
    * pathToFile - Path to a JSON formatted file in the form of <EXAMPLE> TODO 
    * functionProjectLocation - Location of source code on local disk 
    * emailTo - Address to notify on HTML update
    * sendGridKey - Valid API key from [SendGrid](http://sendgrid.com)
3. Run ./Deploy/deployInfra.sh
