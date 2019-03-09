
namespace Faction.Build.Dotnet
{
  public static class Settings {
    // Language ID is automatically set. Don't set it here.
    public static int LanguageId;
    public static string LanguageName = "dotnet";
    public static string AgentConfigName = $"FactionAgent.{LanguageName}.json";
    public static string ModuleConfigName = $"FactionModule.{LanguageName}.json";
    public static string AgentsPath = "/opt/faction/agents/";
    public static string ModulesPath = "/opt/faction/modules/";
  }
}