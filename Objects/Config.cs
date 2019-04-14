using System;
using System.Collections.Generic;

namespace Faction.Build.Dotnet.Objects
{
  public class AgentSubConfig
  {
    public string Name { get; set; }
  }

  public class AgentTransportConfig
  {

    public string Name { get; set; }
    public string TransportTypeGuid { get; set; }
    public string BuildCommand { get; set; }
    public string BuildLocation { get; set; }
  }
  public class AgentTypeConfig
  {
    public string Name { get; set; }
    public string Guid { get; set; }
    public List<string> Authors { get; set; }
    public string BuildCommand { get; set; }
    public string BuildLocation { get; set; }
    public List<AgentSubConfig> OperatingSystems { get; set; }
    public List<AgentSubConfig> Architectures { get; set; }
    public List<AgentSubConfig> Versions { get; set; }
    public List<AgentSubConfig> Formats { get; set; }
    public List<AgentSubConfig> Configurations { get; set; }

    public List<AgentTransportConfig> AgentTransportTypes { get; set; }
    public List<CommandConfig> Commands { get; set; }
  }

  public class CommandParameterConfig
  {
    public CommandParameterConfig()
    {
      Values = new List<string>();
      Required = false;
    }
    public string Name { get; set; }
    public string Help { get; set; }
    public bool Required { get; set; }
    public int? Position { get; set; }
    public List<string> Values { get; set; }
  }
  public class CommandConfig
  {
    public CommandConfig()
    {
      Parameters = new List<CommandParameterConfig>();
      Artifacts = new List<string>();
    }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Help { get; set; }
    public bool OpsecSafe { get; set; }
    public string MitreReference { get; set; }
    public List<CommandParameterConfig> Parameters { get; set; }
    public List<string> Artifacts { get; set; }

  }

  public class ModuleConfig
  {
    public ModuleConfig()
    {
      Commands = new List<CommandConfig>();
    }
    public string Name { get; set; }
    public string Description { get; set; }
    public List<string> Authors { get; set; }
    public string BuildCommand { get; set; }
    public string BuildLocation { get; set; }
    public List<CommandConfig> Commands { get; set; }

  }

  public class BuildConfig
  {
    public string PayloadName;
    public string PayloadKey;
    public string Transport;
    public int BeaconInterval;
    public double Jitter;
    public string ExpirationDate;
    public string Architecture;
    public string OperatingSystem;
    public string Configuration;
    public string Version;
    public bool Debug;
    public string TransportConfiguration;
  }
}