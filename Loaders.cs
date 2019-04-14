using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using RabbitMQ.Client;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

using Faction.Common.Models;
using Faction.Common.Backend.Database;
using Faction.Common.Backend.EventBus.Abstractions;
using Faction.Common.Backend.EventBus.RabbitMQ;
using Faction.Common.Backend.EventBus;

using Faction.Build.Dotnet.Objects;
using Faction.Build.Dotnet.Handlers;

namespace Faction.Build.Dotnet
{
  class Loader
  {
    public static void LoadSelf(FactionRepository dbRepository)
    {
      bool import;
      try
      {
        Language language = dbRepository.GetLanguage(Settings.LanguageName);
        Settings.LanguageId = language.Id;
        import = false;
      }
      catch 
      {
        import = true;
      }

      if (import) {
        Language language = new Language();
        language.Name = Settings.LanguageName;
        dbRepository.Add(language);
        Settings.LanguageId = language.Id;
      }
    }
    public static void LoadModules(FactionRepository dbRepository)
    {
      string[] files = Directory.GetFiles(Settings.ModulesPath, Settings.ModuleConfigName, SearchOption.AllDirectories);
      foreach (string file in files)
      {
        bool import;
        string contents = File.ReadAllText(file);
        JsonSerializerSettings settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Include,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    };
        ModuleConfig moduleConfig = JsonConvert.DeserializeObject<ModuleConfig>(contents);
        try
        {
          Module module = dbRepository.GetModule(moduleConfig.Name, Settings.LanguageName);
          import = false;
        }
        catch
        {
          import = true;
        }
        if (import) {
          Module module = new Module();
          module.Name = moduleConfig.Name;
          module.Description = moduleConfig.Description;
          module.Authors = String.Join(", ", moduleConfig.Authors.ToArray());
          module.BuildLocation = moduleConfig.BuildLocation;
          module.BuildCommand = moduleConfig.BuildCommand;
          module.LanguageId = Settings.LanguageId;
          dbRepository.Add(module);

          foreach (CommandConfig commandConfig in moduleConfig.Commands) {
            Command command = new Command();
            command.Name = commandConfig.Name;
            command.Description = commandConfig.Description;
            command.Help = commandConfig.Help;
            command.MitreReference = commandConfig.MitreReference;
            command.OpsecSafe = commandConfig.OpsecSafe;
            command.ModuleId = module.Id;
            if (commandConfig.Artifacts.Count > 0)
            {
              command.Artifacts = String.Join(",", commandConfig.Artifacts.ToArray());
            }
            dbRepository.Add(command);
            foreach (CommandParameterConfig paramConfig in commandConfig.Parameters)
            {
              CommandParameter param = new CommandParameter();
              param.Name = paramConfig.Name;
              param.CommandId = command.Id;
              param.Help = paramConfig.Help;
              param.Required = paramConfig.Required;
              param.Position = paramConfig.Position;
              param.Values = String.Join(",", paramConfig.Values.ToArray());
              dbRepository.Add(param);
            }
          }
        }
      }
    }
    public static void LoadAgents(FactionRepository dbRepository)
    {
      string[] files = Directory.GetFiles(Settings.AgentsPath, Settings.AgentConfigName, SearchOption.AllDirectories);
      foreach (string file in files)
      {
        string contents = File.ReadAllText(file);
        AgentTypeConfig agentTypeConfig = JsonConvert.DeserializeObject<AgentTypeConfig>(contents);
        AgentType type = dbRepository.GetAgentType(agentTypeConfig.Name);
        
        if (type == null)
        {
          AgentType agentType = new AgentType();
          agentType.Name = agentTypeConfig.Name;
          agentType.Guid = agentTypeConfig.Guid;
          agentType.Authors = String.Join(", ", agentTypeConfig.Authors.ToArray());
          agentType.LanguageId = Settings.LanguageId;
          dbRepository.Add(agentType);

          foreach (AgentSubConfig subConfig in agentTypeConfig.Architectures)
          {
            AgentTypeArchitecture agentTypeArchitecture = new AgentTypeArchitecture();
            agentTypeArchitecture.Name = subConfig.Name;
            agentTypeArchitecture.AgentTypeId = agentType.Id;
            dbRepository.Add(agentTypeArchitecture);
          }

          foreach (AgentSubConfig osConfig in agentTypeConfig.OperatingSystems)
          {
            AgentTypeOperatingSystem agentTypeOperatingSystem = new AgentTypeOperatingSystem();
            agentTypeOperatingSystem.Name = osConfig.Name;
            agentTypeOperatingSystem.AgentTypeId = agentType.Id;
            dbRepository.Add(agentTypeOperatingSystem);
          }

          foreach (AgentSubConfig formatConfig in agentTypeConfig.Formats)
          {
            AgentTypeFormat agentTypeFormat = new AgentTypeFormat();
            agentTypeFormat.Name = formatConfig.Name;
            agentTypeFormat.AgentTypeId = agentType.Id;
            dbRepository.Add(agentTypeFormat);
          }

          foreach (AgentSubConfig versionConfig in agentTypeConfig.Versions)
          {
            AgentTypeVersion agentTypeVersion = new AgentTypeVersion();
            agentTypeVersion.Name = versionConfig.Name;
            agentTypeVersion.AgentTypeId = agentType.Id;
            dbRepository.Add(agentTypeVersion);
          }
          
          foreach (AgentSubConfig ConfigurationConfig in agentTypeConfig.Configurations)
          {
            AgentTypeConfiguration agentTypeConfiguration = new AgentTypeConfiguration();
            agentTypeConfiguration.Name = ConfigurationConfig.Name;
            agentTypeConfiguration.AgentTypeId = agentType.Id;
            dbRepository.Add(agentTypeConfiguration);
          }


          foreach (AgentTransportConfig agentTransportConfig in agentTypeConfig.AgentTransportTypes)
          {
            AgentTransportType agentTransportType = new AgentTransportType();
            agentTransportType.Name = agentTransportConfig.Name;
            agentTransportType.TransportTypeGuid = agentTransportConfig.TransportTypeGuid;
            agentTransportType.BuildCommand = agentTransportConfig.BuildCommand;
            agentTransportType.BuildLocation = agentTransportConfig.BuildLocation;
            agentTransportType.AgentTypeId = agentType.Id;
            dbRepository.Add(agentTransportType);
          }

          foreach (CommandConfig commandConfig in agentTypeConfig.Commands)
          {
            Command command = new Command();
            command.Name = commandConfig.Name;
            command.Description = commandConfig.Description;
            command.Help = commandConfig.Help;
            command.MitreReference = commandConfig.MitreReference;
            command.OpsecSafe = commandConfig.OpsecSafe;
            command.AgentTypeId = agentType.Id;
            if (commandConfig.Artifacts.Count > 0)
            {
              command.Artifacts = String.Join(",", commandConfig.Artifacts.ToArray());
            }
            dbRepository.Add(command);
            foreach (CommandParameterConfig paramConfig in commandConfig.Parameters)
            {
              CommandParameter param = new CommandParameter();
              param.Name = paramConfig.Name;
              param.CommandId = command.Id;
              param.Help = paramConfig.Help;
              param.Required = paramConfig.Required;
              param.Position = paramConfig.Position;
              param.Values = String.Join(",", paramConfig.Values.ToArray());
              dbRepository.Add(param);
            }
          }
        }

      }
    }
  }
}