Notes for deploying dapr service on azure

- Make sure to use lowercase when naming the service

1. Create dockerfile for service
2. update main.yaml
3. Push to main
4. Create new container in azure, with the container from the container regestry
5. enable dapr & ingress (port 80)
6. In the app container enviroment, select dapr and select the secretservice
7. add scope for the  new app
8. do the same for the statestore
9. in the container app: go to Identity
10. Enable system assigned identity
11. copy principal id
12. go to the daprvault (keyvault)
13. go to  access policies
14. click create and use the principle id
15. restart the application and test it out, checkout the logs for any errors
