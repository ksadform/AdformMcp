namespace AdformMcp;

public static class Prompt
{
    public static string ImplementAmplitude(
        string id,
        string instruction,
        string amplitudePageData,
        string eventsTaxonomyData,
        string propertiesTaxonomyData,
        string implementationPatternData)
    {
        return $@"# Amplitude Events Implementation

## Instructions
{instruction}

---

## Amplitude Event Data (Page ID: {id})
{amplitudePageData}

---

## Events Taxonomy Reference
{eventsTaxonomyData}

---

## Properties Taxonomy Reference
{propertiesTaxonomyData}

---

## Implementation Patterns & Best Practices
{implementationPatternData}

---

## Next Steps
1. **Hold for Confirmation**: Wait for the user to give the go-ahead before proceeding.
2. **Identify Events**: Note that the data may contain several different Amplitude events.
3. **Clarify Scope**: Ask the user if they want to set up all detected events or just a specific one.
4. **Display Options**: Present the list of event names in a clean, easy-to-read format.
5. **Follow Patterns**: Use the implementation patterns provided above to ensure consistency.
6. **Validate Properties**: Cross-reference event properties with the properties taxonomy to ensure correct naming and types.

";
    }
}
