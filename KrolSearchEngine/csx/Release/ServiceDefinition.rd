<?xml version="1.0" encoding="utf-8"?>
<serviceModel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" name="KrolSearchEngine" generation="1" functional="0" release="0" Id="6d2e7449-917e-447a-aef6-2f084e73608e" dslVersion="1.2.0.0" xmlns="http://schemas.microsoft.com/dsltools/RDSM">
  <groups>
    <group name="KrolSearchEngineGroup" generation="1" functional="0" release="0">
      <componentports>
        <inPort name="SearchEngineWebRole:Endpoint1" protocol="http">
          <inToChannel>
            <lBChannelMoniker name="/KrolSearchEngine/KrolSearchEngineGroup/LB:SearchEngineWebRole:Endpoint1" />
          </inToChannel>
        </inPort>
      </componentports>
      <settings>
        <aCS name="SearchEngineWebRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/KrolSearchEngine/KrolSearchEngineGroup/MapSearchEngineWebRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="SearchEngineWebRoleInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/KrolSearchEngine/KrolSearchEngineGroup/MapSearchEngineWebRoleInstances" />
          </maps>
        </aCS>
        <aCS name="SearchEngineWorkerRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/KrolSearchEngine/KrolSearchEngineGroup/MapSearchEngineWorkerRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="SearchEngineWorkerRoleInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/KrolSearchEngine/KrolSearchEngineGroup/MapSearchEngineWorkerRoleInstances" />
          </maps>
        </aCS>
      </settings>
      <channels>
        <lBChannel name="LB:SearchEngineWebRole:Endpoint1">
          <toPorts>
            <inPortMoniker name="/KrolSearchEngine/KrolSearchEngineGroup/SearchEngineWebRole/Endpoint1" />
          </toPorts>
        </lBChannel>
      </channels>
      <maps>
        <map name="MapSearchEngineWebRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/KrolSearchEngine/KrolSearchEngineGroup/SearchEngineWebRole/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapSearchEngineWebRoleInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/KrolSearchEngine/KrolSearchEngineGroup/SearchEngineWebRoleInstances" />
          </setting>
        </map>
        <map name="MapSearchEngineWorkerRole:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/KrolSearchEngine/KrolSearchEngineGroup/SearchEngineWorkerRole/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapSearchEngineWorkerRoleInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/KrolSearchEngine/KrolSearchEngineGroup/SearchEngineWorkerRoleInstances" />
          </setting>
        </map>
      </maps>
      <components>
        <groupHascomponents>
          <role name="SearchEngineWebRole" generation="1" functional="0" release="0" software="C:\Users\rhenvar\Documents\Visual Studio 2015\Projects\KrolSearchEngine\KrolSearchEngine\csx\Release\roles\SearchEngineWebRole" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaIISHost.exe " memIndex="-1" hostingEnvironment="frontendadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="Endpoint1" protocol="http" portRanges="80" />
            </componentports>
            <settings>
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;SearchEngineWebRole&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;SearchEngineWebRole&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;r name=&quot;SearchEngineWorkerRole&quot; /&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/KrolSearchEngine/KrolSearchEngineGroup/SearchEngineWebRoleInstances" />
            <sCSPolicyUpdateDomainMoniker name="/KrolSearchEngine/KrolSearchEngineGroup/SearchEngineWebRoleUpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/KrolSearchEngine/KrolSearchEngineGroup/SearchEngineWebRoleFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
        <groupHascomponents>
          <role name="SearchEngineWorkerRole" generation="1" functional="0" release="0" software="C:\Users\rhenvar\Documents\Visual Studio 2015\Projects\KrolSearchEngine\KrolSearchEngine\csx\Release\roles\SearchEngineWorkerRole" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaWorkerHost.exe " memIndex="-1" hostingEnvironment="consoleroleadmin" hostingEnvironmentVersion="2">
            <settings>
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;SearchEngineWorkerRole&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;SearchEngineWebRole&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;/r&gt;&lt;r name=&quot;SearchEngineWorkerRole&quot; /&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/KrolSearchEngine/KrolSearchEngineGroup/SearchEngineWorkerRoleInstances" />
            <sCSPolicyUpdateDomainMoniker name="/KrolSearchEngine/KrolSearchEngineGroup/SearchEngineWorkerRoleUpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/KrolSearchEngine/KrolSearchEngineGroup/SearchEngineWorkerRoleFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
      </components>
      <sCSPolicy>
        <sCSPolicyUpdateDomain name="SearchEngineWebRoleUpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyUpdateDomain name="SearchEngineWorkerRoleUpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyFaultDomain name="SearchEngineWebRoleFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyFaultDomain name="SearchEngineWorkerRoleFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyID name="SearchEngineWebRoleInstances" defaultPolicy="[1,1,1]" />
        <sCSPolicyID name="SearchEngineWorkerRoleInstances" defaultPolicy="[1,1,1]" />
      </sCSPolicy>
    </group>
  </groups>
  <implements>
    <implementation Id="4480560c-8238-4446-b17d-f6f6896f5dfd" ref="Microsoft.RedDog.Contract\ServiceContract\KrolSearchEngineContract@ServiceDefinition">
      <interfacereferences>
        <interfaceReference Id="511b5358-b4cb-473f-8604-b87c2c5acae9" ref="Microsoft.RedDog.Contract\Interface\SearchEngineWebRole:Endpoint1@ServiceDefinition">
          <inPort>
            <inPortMoniker name="/KrolSearchEngine/KrolSearchEngineGroup/SearchEngineWebRole:Endpoint1" />
          </inPort>
        </interfaceReference>
      </interfacereferences>
    </implementation>
  </implements>
</serviceModel>