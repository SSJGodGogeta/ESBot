namespace ESBot.Domain.Enums;

public enum EMessageRole
{
    // Nutzer input
    User,
    // KI Output
    Bot,
    // KI Monitor / Fine Tuner
    // Bearbeitet nochmal den Output, indem es der KI zusätzliche Prompts gibt
    System
}