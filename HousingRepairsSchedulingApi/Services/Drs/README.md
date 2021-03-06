# DRS SOAP Service Reference

### Generating code to communicate with DRS SOAP API
How to consume a SOAP API from .Net is outlined in this [article](https://docs.microsoft.com/en-us/dotnet/core/additional-tools/dotnet-svcutil-guide).

The specific steps for DRS are as follows:
1. Install [dotnet-svcutil](https://www.nuget.org/packages/dotnet-svcutil).
```c#
dotnet tool install --global dotnet-svcutil
```
2. Get WSDL file

DRS publishes it’s WSDL file via the following URL: 

```
https://<INSTALLATION WEB ADDRESS>/OTWebServiceGateway_<INSTANCENAME>/ws/soap?wsdl
```

where `<INSTALLATION WEB ADDRESS>` and `<INSTALLATION WEB ADDRESS>` should be replaced.  

In the same directory as this README is a WSDL file, `soap.wsdl`, for DRS.
3. Generate Service Reference

`dotnet-svcutil` is used to generate code for a WSDL file.
Run the following command within the same directory as this README.
```shell
dotnet-svcutil --namespace "*,HousingRepairsSchedulingApi.Services.Drs" soap.wsdl
```

The `--namespace` option specifies a mapping from the WSDL namespace to a .Net namespace.
Using `*` for the WSDL namespace results in all namespaces being mapped to the corresponding .Net namespace.

See `dotnet-svcutil --help` for more info

Once the above command completes, the generated service reference will be located in a `ServiceReference` subfolder.

The code generated will contain a `SOAP` interface and `SOAPClient` class, which implements the interface.
This maintains low coupling and high cohesion which results in usages being highly testable and dependency injectable.

### Creating a Client

Use the following to create a SOAPClient, ensuring the replace `<INSTALLATION WEB ADDRESS>` and `<INSTALLATION WEB ADDRESS>`:

```c#
var binding = new BasicHttpBinding();
var endpoint = new EndpointAddress(new Uri("http://<INSTALLATION WEB ADDRESS>/OTWebServiceGateway_<INSTANCENAME>/ws/soap_560"));
SOAP soapClient = new SOAPClient(binding, endpoint);
```

### Making Requests

The generated code requires modification to make requests which meet DRS requirements.

Each request takes a parameter, i.e. `SOAP.openSessionAsync` has a `openSession` parameter.
The parameter classes have been generated with XML namespace values of `http://autogenerated.OTWebServiceApi.xmbrace.com/`, i.e.
```c#
[System.ServiceModel.MessageContractAttribute(WrapperName="openSession", WrapperNamespace="http://autogenerated.OTWebServiceApi.xmbrace.com/", IsWrapped=true)]
public partial class openSession
{
    
    [System.ServiceModel.MessageBodyMemberAttribute(Name="openSession", Namespace="http://autogenerated.OTWebServiceApi.xmbrace.com/", Order=0)]
    public HousingRepairsSchedulingApi.Services.Drs.xmbOpenSession openSession1;
    ...
}
```

These need to be removed to ensure the request is of the correct format for DRS.
The above parameter class would become:
```c#
[System.ServiceModel.MessageContractAttribute(WrapperName="openSession", WrapperNamespace="", IsWrapped=true)]
public partial class openSession
{
    
    [System.ServiceModel.MessageBodyMemberAttribute(Name="openSession", Namespace="", Order=0)]
    public HousingRepairsSchedulingApi.Services.Drs.xmbOpenSession openSession1;
    ...
}
```

Without these changes DRS will respond to requests with the following error:
```
Unmarshalling Error: Namespace URIs and local names to the unmarshaller needs to be interned.
```

An `openSession` request can be made using the SOAPClient as follows:
```c#
var openSession = new xmbOpenSession { login = "<LOGIN>", password = "<PASSWORD" };
var openSessionResponse = await soapClient.openSessionAsync(new openSession(openSession));
var sessionId = openSessionResponse.@return.sessionId;
```

Other requests can be made in a similar, equivalent way.
