namespace AdformMcp;

public static class Prompt
{
    public static string ImplementAmplitude(string id, string instruction, string amplitudeData)
    {
        return $@"# Amplitude Events Implementation

## Instructions
{instruction}

---

## Amplitude Data from Confluence (Page ID: {id})
{amplitudeData}

---

You now have both the implementation guide and the specific Amplitude event data needed to implement the tracking.
Hold for Confirmation: Wait for the user to give the go-ahead before proceeding.
Identify Events: Note that the data may contain several different Amplitude events.
Clarify Scope: Ask the user if they want to set up all detected events or just a specific one.
Display Options: Present the list of event names in a clean, easy-to-read format.

";
    }

}
