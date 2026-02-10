# Amplitude Tracking Implementation Guide

> **⚠️ IMPORTANT: Do NOT generate any summary files, documentation files, or any type of `.md` files during implementation. Only implement the tracking code directly in the relevant source files.**

## Table of Contents

1. [Overview](#overview)
2. [Prerequisites](#prerequisites)
3. [Understanding the Data Structure](#understanding-the-data-structure)
4. [Implementation Steps](#implementation-steps)
5. [Constants Management](#constants-management)
6. [Code Examples](#code-examples)
7. [Best Practices](#best-practices)

---

## Overview

The AAP Frontend uses Amplitude for usage tracking through a centralized `UsageTracker` abstraction layer. Events are tracked using the `@amplitude/analytics-browser` SDK with proper categorization and properties.

**Key Architecture Components:**

-   `UsageTracker` class: Abstraction over Amplitude API (`@aap/bootstrap/src/modules/usageTracking/UsageTracker.js`)
-   `useUsageTracker` hook: React hook to access tracker instance
-   Constants: Centralized event names, categories, and property values
-   Applet definitions: Base tracking properties defined at applet level

---

## Prerequisites

### Required Knowledge

-   React hooks (specifically `useUsageTracker`)
-   JavaScript ES6+ syntax
-   Understanding of the component where tracking will be implemented

### Required Imports

```javascript
import {
    useUsageTracker,
    USAGE_TRACKING_EVENT_NAMES,
    USAGE_TRACKING_EVENT_CATEGORIES,
    USAGE_TRACKING_EVENT_PRODUCT_GROUPS,
    USAGE_TRACKING_EVENT_PRODUCT_APPS,
    USAGE_TRACKING_EVENT_LAYOUT,
    USAGE_TRACKING_EVENT_WORKFLOWS,
    USAGE_TRACKING_EVENT_ENTITY_TYPES,
    USAGE_TRACKING_EVENT_APPLET_TYPES,
} from '@aap/bootstrap';
```

---

## Understanding the Data Structure

### Input JSON Format

The `amplitude.input.json` file contains an array of event definitions:

```json
{
    "amplitudeEvents": [
        {
            "recordId": 32,
            "productGroup": "DSP",
            "what": "When applet is opened",
            "where": "Inventory Deal Groups Selection List Applet",
            "uiApplet": "adform-buyside-dealGroup-marketplace-entitySelection-list",
            "properties": {
                "type": ["create", "edit"],
                "layout": "sidePanel",
                "openedFrom": null,
                "appletName": null,
                "entityType": "dealGroup"
            },
            "productApp": "dealGroups",
            "workflow": null,
            "eventName": "Applet opened",
            "eventNameGitz": "APPLET_OPENED",
            "eventType": "Navigation"
        }
    ]
}
```

### Field Mapping

| JSON Field      | Usage                     | Constant/Value                                              |
| --------------- | ------------------------- | ----------------------------------------------------------- |
| `eventName`     | Human-readable event name | Use as display name in tracking                             |
| `eventNameGitz` | Constant key              | Map to `USAGE_TRACKING_EVENT_NAMES.{eventNameGitz}`         |
| `eventType`     | Event category            | Map to `USAGE_TRACKING_EVENT_CATEGORIES.{UPPERCASE}`        |
| `productGroup`  | Product group             | Map to `USAGE_TRACKING_EVENT_PRODUCT_GROUPS.{VALUE}`        |
| `productApp`    | Product application       | Use as string or map to `USAGE_TRACKING_EVENT_PRODUCT_APPS` |
| `workflow`      | Workflow identifier       | Map to `USAGE_TRACKING_EVENT_WORKFLOWS.{VALUE}` if not null |
| `uiApplet`      | Applet name               | Use for applet identification                               |
| `properties`    | Event properties          | Extract and map individual properties                       |

---

## Implementation Steps

### Step 1: Verify/Add Constants

Before implementing tracking, ensure all required constants exist:

> **Important Rule:** Always check if a constant already exists in the appropriate constants file before adding a new one. If the constant exists, use it directly. Only add a new constant if it doesn't already exist in the file.

#### 1.1 Event Names

**Location:** `@aap/bootstrap/src/constants/usageTracking/eventNames.js`

```javascript
export const USAGE_TRACKING_EVENT_NAMES = {
    APPLET_OPENED: 'Applet opened',
    APPLET_CLOSED: 'Applet closed',
    // Add new event names if needed
};
```

#### 1.2 Event Categories

**Location:** `@aap/bootstrap/src/constants/usageTracking/eventCategories.js`

```javascript
export const USAGE_TRACKING_EVENT_CATEGORIES = {
    DISCOVERY: 'Discovery',
    NAVIGATION: 'Navigation',
    PLATFORM: 'Platform',
    REVENUE: 'Revenue',
    OPTIMIZATION: 'Optimization',
    RULES_SETUP: 'Rules Set-up',
    SELECTION: 'Selection',
    PRODUCT_SETUP: 'Product Set-up',
};
```

#### 1.3 Product Groups

**Location:** `@aap/bootstrap/src/constants/usageTracking/eventProductGroups.js`

```javascript
export const USAGE_TRACKING_EVENT_PRODUCT_GROUPS = {
    DSP: 'DSP',
    TPAS: 'TPAS',
    DMP: 'DMP',
    SSP: 'SSP',
    PPAS: 'PPAS',
    DCOPRO: 'DCO Pro',
    GENERAL: 'General',
    IAM: 'User Management',
    BUYSIDE: 'Buy Side',
    REPORTS: 'Reports',
};
```

#### 1.4 Workflows

**Location:** `@aap/bootstrap/src/constants/usageTracking/eventWorkflows.js`

Add workflow constant if specified in your data:

```javascript
export const USAGE_TRACKING_EVENT_WORKFLOWS = {
    CREATE_DEAL_GROUP: 'createDealGroup',
    EDIT_DEAL_GROUP: 'editDealGroup',
    // Add new workflows as needed
};
```

#### 1.5 Entity Types

**Location:** `@aap/bootstrap/src/constants/usageTracking/eventEntityTypes.js`

```javascript
export const USAGE_TRACKING_EVENT_ENTITY_TYPES = {
    DEAL: 'deal',
    DEAL_GROUP: 'dealGroup',
    // Add new entity types as needed
};
```

#### 1.6 Layouts

**Location:** `@aap/bootstrap/src/constants/usageTracking/eventLayout.js`

```javascript
export const USAGE_TRACKING_EVENT_LAYOUT = {
    LIST: 'List',
    SIDEPANEL: 'SidePanel',
    DIALOG: 'Dialog',
    PAGE: 'Page',
    QUICK_PREVIEW: 'QuickPreview',
    DASHBOARD: 'Dashboard',
};
```

### Step 2: Update Applet Definition (Optional)

If the tracking applies to an entire applet, add base tracking properties to the applet definition:

**Location:** `{applet-path}/definition.js`

```javascript
export default {
    name: 'adform-buyside-dealGroup-marketplace-entitySelection-list',
    loader: () => import('./index'),
    path: '/deal-groups/selection',
    usageTracking: {
        productGroup: USAGE_TRACKING_EVENT_PRODUCT_GROUPS.DSP,
        productApp: 'dealGroups',
        productComponent: 'DealGroupsSelection',
        layout: USAGE_TRACKING_EVENT_LAYOUT.SIDEPANEL,
    },
    // ... other definition properties
};
```

**Note:** Properties defined in `usageTracking` are automatically included in all events tracked within that applet.

### Step 3: Implement Tracking in Component

#### 3.1 Import Required Dependencies

```javascript
import {
    useUsageTracker,
    USAGE_TRACKING_EVENT_NAMES,
    USAGE_TRACKING_EVENT_CATEGORIES,
    USAGE_TRACKING_EVENT_PRODUCT_GROUPS,
    USAGE_TRACKING_EVENT_LAYOUT,
    USAGE_TRACKING_EVENT_WORKFLOWS,
    USAGE_TRACKING_EVENT_ENTITY_TYPES,
} from '@aap/bootstrap';
```

#### 3.2 Initialize the Hook

```javascript
const MyComponent = ({ type, openedFrom }) => {
    const usageTracker = useUsageTracker();

    // ... component logic
};
```

#### 3.3 Track Events

**Pattern A: Track on Component Mount (useEffect)**

```javascript
useEffect(() => {
    usageTracker.trackEvent(USAGE_TRACKING_EVENT_NAMES.APPLET_OPENED, {
        category: USAGE_TRACKING_EVENT_CATEGORIES.NAVIGATION,
        productGroup: USAGE_TRACKING_EVENT_PRODUCT_GROUPS.DSP,
        layout: USAGE_TRACKING_EVENT_LAYOUT.SIDEPANEL,
        type: type, // 'create' or 'edit'
        openedFrom: openedFrom || 'DirectUrl',
        entityType: USAGE_TRACKING_EVENT_ENTITY_TYPES.DEAL_GROUP,
    });
}, []); // Empty dependency array for mount only
```

**Pattern B: Track on User Action (Event Handler)**

```javascript
const handleClose = () => {
    usageTracker.trackEvent(USAGE_TRACKING_EVENT_NAMES.APPLET_CLOSED, {
        category: USAGE_TRACKING_EVENT_CATEGORIES.NAVIGATION,
        productGroup: USAGE_TRACKING_EVENT_PRODUCT_GROUPS.DSP,
        layout: USAGE_TRACKING_EVENT_LAYOUT.SIDEPANEL,
        type: type,
        entityType: USAGE_TRACKING_EVENT_ENTITY_TYPES.DEAL_GROUP,
    });

    // Execute actual close logic
    onClose();
};
```

**Pattern C: Track Multiple Events**

```javascript
const trackMultipleEvents = () => {
    usageTracker.trackEvents([
        {
            eventName: USAGE_TRACKING_EVENT_NAMES.APPLET_OPENED,
            eventProperties: {
                category: USAGE_TRACKING_EVENT_CATEGORIES.NAVIGATION,
                productGroup: USAGE_TRACKING_EVENT_PRODUCT_GROUPS.DSP,
            },
        },
        {
            eventName: USAGE_TRACKING_EVENT_NAMES.FORM_LOADED,
            eventProperties: {
                category: USAGE_TRACKING_EVENT_CATEGORIES.PLATFORM,
                productGroup: USAGE_TRACKING_EVENT_PRODUCT_GROUPS.DSP,
            },
        },
    ]);
};
```

### Step 4: Handle Dynamic Properties

If properties can have multiple values (e.g., `type: ["create", "edit"]`), handle them based on runtime state:

```javascript
const MyComponent = ({ mode }) => {
    const usageTracker = useUsageTracker();

    // Determine the type based on component state/props
    const getType = () => {
        if (mode === 'new') return 'create';
        if (mode === 'update') return 'edit';
        return null;
    };

    useEffect(() => {
        usageTracker.trackEvent(USAGE_TRACKING_EVENT_NAMES.APPLET_OPENED, {
            category: USAGE_TRACKING_EVENT_CATEGORIES.NAVIGATION,
            productGroup: USAGE_TRACKING_EVENT_PRODUCT_GROUPS.DSP,
            type: getType(),
            // ... other properties
        });
    }, []);
};
```

### Step 5: Handle Conditional Properties

Properties with `null` values in the JSON should only be included when they have actual values:

```javascript
const trackEventWithConditionalProps = () => {
    const baseProps = {
        category: USAGE_TRACKING_EVENT_CATEGORIES.NAVIGATION,
        productGroup: USAGE_TRACKING_EVENT_PRODUCT_GROUPS.DSP,
        layout: USAGE_TRACKING_EVENT_LAYOUT.SIDEPANEL,
    };

    // Only add workflow if it exists
    const eventProps = workflow
        ? { ...baseProps, workflow: USAGE_TRACKING_EVENT_WORKFLOWS.EDIT_DEAL_GROUP }
        : baseProps;

    // Only add openedFrom if it's provided
    if (openedFrom) {
        eventProps.openedFrom = openedFrom;
    }

    usageTracker.trackEvent(USAGE_TRACKING_EVENT_NAMES.APPLET_OPENED, eventProps);
};
```

---

## Constants Management

### Adding New Constants

If your JSON data includes values not present in existing constants:

1. **Check if the constant already exists** in the appropriate constants file
2. **If it exists**, use that constant directly
3. **If it doesn't exist**, proceed with adding it:
   - **Identify the constant file** based on the property type
   - **Add the constant** following the existing pattern
   - **Export it** from the index file if not auto-exported

> **Critical:** Always verify the constant doesn't already exist before adding a new one to avoid duplication.

**Example: Adding a new workflow**

```javascript
// File: @aap/bootstrap/src/constants/usageTracking/eventWorkflows.js
export const USAGE_TRACKING_EVENT_WORKFLOWS = {
    // ... existing workflows
    BULK_EDIT_DEAL_GROUPS: 'bulkEditDealGroups', // New addition
};
```

### Constant Naming Conventions

-   **Event Names**: Use `SCREAMING_SNAKE_CASE` with descriptive names
-   **Values**: Match the JSON `eventName` field exactly (with proper casing)
-   **Product Apps**: Use descriptive names or match backend terminology
-   **Workflows**: Use `camelCase` for workflow identifiers

---

## Code Examples

### Example 1: Basic Applet Opened/Closed Tracking

```javascript
import { useEffect } from 'react';
import {
    useUsageTracker,
    USAGE_TRACKING_EVENT_NAMES,
    USAGE_TRACKING_EVENT_CATEGORIES,
    USAGE_TRACKING_EVENT_PRODUCT_GROUPS,
    USAGE_TRACKING_EVENT_LAYOUT,
    USAGE_TRACKING_EVENT_ENTITY_TYPES,
} from '@aap/bootstrap';

const DealGroupSelectionApplet = ({ type, onClose }) => {
    const usageTracker = useUsageTracker();

    // Track applet opened on mount
    useEffect(() => {
        usageTracker.trackEvent(USAGE_TRACKING_EVENT_NAMES.APPLET_OPENED, {
            category: USAGE_TRACKING_EVENT_CATEGORIES.NAVIGATION,
            productGroup: USAGE_TRACKING_EVENT_PRODUCT_GROUPS.DSP,
            productApp: 'dealGroups',
            layout: USAGE_TRACKING_EVENT_LAYOUT.SIDEPANEL,
            type: type,
            entityType: USAGE_TRACKING_EVENT_ENTITY_TYPES.DEAL_GROUP,
        });
    }, []);

    // Track applet closed
    const handleClose = () => {
        usageTracker.trackEvent(USAGE_TRACKING_EVENT_NAMES.APPLET_CLOSED, {
            category: USAGE_TRACKING_EVENT_CATEGORIES.NAVIGATION,
            productGroup: USAGE_TRACKING_EVENT_PRODUCT_GROUPS.DSP,
            productApp: 'dealGroups',
            layout: USAGE_TRACKING_EVENT_LAYOUT.SIDEPANEL,
            type: type,
            entityType: USAGE_TRACKING_EVENT_ENTITY_TYPES.DEAL_GROUP,
        });

        onClose();
    };

    return (
        <div>
            {/* Component content */}
            <button onClick={handleClose}>Close</button>
        </div>
    );
};
```

### Example 2: Custom Hook for Amplitude Tracking

Create a reusable hook for complex tracking logic:

```javascript
// hooks/useAmplitudeTracking.js
import {
    useUsageTracker,
    USAGE_TRACKING_EVENT_NAMES,
    USAGE_TRACKING_EVENT_CATEGORIES,
    USAGE_TRACKING_EVENT_PRODUCT_GROUPS,
} from '@aap/bootstrap';

const useAmplitudeTracking = ({ eventConfig }) => {
    const usageTracker = useUsageTracker();

    const baseProps = {
        productGroup: eventConfig.productGroup || USAGE_TRACKING_EVENT_PRODUCT_GROUPS.DSP,
        category: eventConfig.category || USAGE_TRACKING_EVENT_CATEGORIES.NAVIGATION,
        productApp: eventConfig.productApp,
        layout: eventConfig.layout,
    };

    const trackAppletOpened = (additionalProps = {}) => {
        usageTracker.trackEvent(USAGE_TRACKING_EVENT_NAMES.APPLET_OPENED, {
            ...baseProps,
            ...additionalProps,
        });
    };

    const trackAppletClosed = (additionalProps = {}) => {
        usageTracker.trackEvent(USAGE_TRACKING_EVENT_NAMES.APPLET_CLOSED, {
            ...baseProps,
            ...additionalProps,
        });
    };

    const trackActionButton = (additionalProps = {}) => {
        usageTracker.trackEvent(USAGE_TRACKING_EVENT_NAMES.ACTION_BUTTON_CLICKED, {
            ...baseProps,
            ...additionalProps,
        });
    };

    return {
        trackAppletOpened,
        trackAppletClosed,
        trackActionButton,
    };
};

export default useAmplitudeTracking;
```

**Usage:**

```javascript
const MyComponent = () => {
    const { trackAppletOpened, trackAppletClosed, trackActionButton } = useAmplitudeTracking({
        eventConfig: {
            productGroup: USAGE_TRACKING_EVENT_PRODUCT_GROUPS.DSP,
            category: USAGE_TRACKING_EVENT_CATEGORIES.NAVIGATION,
            productApp: 'dealGroups',
            layout: USAGE_TRACKING_EVENT_LAYOUT.SIDEPANEL,
        },
    });

    useEffect(() => {
        trackAppletOpened({ type: 'create', entityType: 'dealGroup' });
    }, []);

    // ... rest of component
};
```

### Example 3: Processing Multiple Events from JSON

```javascript
// utils/amplitudeProcessor.js
import {
    USAGE_TRACKING_EVENT_NAMES,
    USAGE_TRACKING_EVENT_CATEGORIES,
    USAGE_TRACKING_EVENT_PRODUCT_GROUPS,
    USAGE_TRACKING_EVENT_LAYOUT,
} from '@aap/bootstrap';

// Map JSON eventType to constant
const getCategoryConstant = (eventType) => {
    const categoryMap = {
        Navigation: USAGE_TRACKING_EVENT_CATEGORIES.NAVIGATION,
        Platform: USAGE_TRACKING_EVENT_CATEGORIES.PLATFORM,
        Discovery: USAGE_TRACKING_EVENT_CATEGORIES.DISCOVERY,
        Revenue: USAGE_TRACKING_EVENT_CATEGORIES.REVENUE,
        Optimization: USAGE_TRACKING_EVENT_CATEGORIES.OPTIMIZATION,
    };
    return categoryMap[eventType] || USAGE_TRACKING_EVENT_CATEGORIES.PLATFORM;
};

// Map JSON layout to constant
const getLayoutConstant = (layout) => {
    if (!layout) return null;

    const layoutMap = {
        sidePanel: USAGE_TRACKING_EVENT_LAYOUT.SIDEPANEL,
        dialog: USAGE_TRACKING_EVENT_LAYOUT.DIALOG,
        page: USAGE_TRACKING_EVENT_LAYOUT.PAGE,
        list: USAGE_TRACKING_EVENT_LAYOUT.LIST,
    };
    return layoutMap[layout];
};

// Map JSON productGroup to constant
const getProductGroupConstant = (productGroup) => {
    return USAGE_TRACKING_EVENT_PRODUCT_GROUPS[productGroup] || productGroup;
};

// Convert JSON event to tracking properties
export const processAmplitudeEvent = (jsonEvent) => {
    const baseProps = {
        category: getCategoryConstant(jsonEvent.eventType),
        productGroup: getProductGroupConstant(jsonEvent.productGroup),
    };

    // Add optional properties
    if (jsonEvent.productApp) {
        baseProps.productApp = jsonEvent.productApp;
    }

    if (jsonEvent.properties?.layout) {
        baseProps.layout = getLayoutConstant(jsonEvent.properties.layout);
    }

    if (jsonEvent.workflow) {
        baseProps.workflow = jsonEvent.workflow;
    }

    // Add custom properties from JSON
    Object.entries(jsonEvent.properties || {}).forEach(([key, value]) => {
        if (value !== null && key !== 'layout') {
            baseProps[key] = value;
        }
    });

    return {
        eventName: USAGE_TRACKING_EVENT_NAMES[jsonEvent.eventNameGitz],
        eventProperties: baseProps,
    };
};
```

### Example 4: Tracking with Error Handling

```javascript
import {
    useUsageTracker,
    USAGE_TRACKING_EVENT_NAMES,
    USAGE_TRACKING_EVENT_CATEGORIES,
    USAGE_TRACKING_ERROR_TYPES,
} from '@aap/bootstrap';

const MyComponent = () => {
    const usageTracker = useUsageTracker();

    const handleSaveWithTracking = async () => {
        try {
            await saveData();

            // Track success
            usageTracker.trackEvent(USAGE_TRACKING_EVENT_NAMES.DEAL_SAVED, {
                category: USAGE_TRACKING_EVENT_CATEGORIES.REVENUE,
                productGroup: USAGE_TRACKING_EVENT_PRODUCT_GROUPS.DSP,
            });
        } catch (error) {
            // Track error
            usageTracker.trackEvent(USAGE_TRACKING_EVENT_NAMES.ERROR_SHOWN, {
                category: USAGE_TRACKING_EVENT_CATEGORIES.PLATFORM,
                productGroup: USAGE_TRACKING_EVENT_PRODUCT_GROUPS.DSP,
                errorType: USAGE_TRACKING_ERROR_TYPES.TOASTER,
                errorMessage: error.message,
            });
        }
    };

    return <button onClick={handleSaveWithTracking}>Save</button>;
};
```

---

## Best Practices

### 1. Constant Verification

-   ✅ **Always check if a constant exists before adding a new one**
-   ✅ Search the appropriate constants file for existing values
-   ✅ Use existing constants to maintain consistency
-   ❌ Don't add duplicate constants
-   ❌ Don't assume a constant doesn't exist without checking

### 2. Event Naming

-   ✅ Use existing `USAGE_TRACKING_EVENT_NAMES` constants
-   ✅ Follow the pattern: `{ENTITY}_{ACTION}` (e.g., `APPLET_OPENED`, `DEAL_SAVED`)
-   ❌ Don't hardcode event names as strings

### 3. Property Consistency

-   ✅ Always include `category` and `productGroup`
-   ✅ Use constants for all property values when available
-   ✅ Include `layout` for UI-related events
-   ❌ Don't mix different naming conventions (camelCase vs snake_case)

### 4. Code Organization

-   ✅ Create custom hooks for complex tracking logic
-   ✅ Define base properties at the top of the component
-   ✅ Group related tracking calls together
-   ❌ Don't scatter tracking calls throughout the component

### 5. Performance

-   ✅ Track events asynchronously (already handled by `UsageTracker`)
-   ✅ Use `useEffect` with proper dependencies for mount/unmount tracking
-   ✅ Avoid tracking in render methods
-   ❌ Don't track events in loops or high-frequency callbacks

### 6. Testing

-   ✅ Mock `useUsageTracker` in component tests
-   ✅ Verify tracking calls with correct properties
-   ❌ Don't write test cases specifically for amplitude tracking (as per project guidelines)

### 7. Properties with Multiple Values

When a property can have multiple values (e.g., `type: ["create", "edit"]`):

-   ✅ Determine the actual value at runtime based on component state
-   ✅ Use conditional logic or prop values
-   ❌ Don't pass arrays as property values

### 8. Null/Undefined Properties

-   ✅ Only include properties that have meaningful values
-   ✅ Use conditional spreading or if statements to add optional properties
-   ❌ Don't pass `null` or `undefined` as property values

### 9. Workflow Tracking

-   ✅ Include `workflow` property for multi-step processes
-   ✅ Use consistent workflow identifiers across related events
-   ✅ Document workflow purpose in code comments

### 10. appletDefinition Integration

-   ✅ Define common properties in applet `usageTracking` configuration
-   ✅ Let the framework automatically merge these properties
-   ❌ Don't duplicate properties already defined in applet definition

### 11. Error Tracking

-   ✅ Track errors with `ERROR_SHOWN` event
-   ✅ Include `errorType` and `errorMessage`
-   ✅ Track errors at the point where they're shown to users

---

## Implementation Checklist

Use this checklist when implementing tracking from `amplitude.input.json`:

-   [ ] Load and review the `amplitude.input.json` file
-   [ ] **Check if required constants already exist in the constants files**
-   [ ] If constants exist, use them directly
-   [ ] If constants don't exist, add them following the existing patterns
-   [ ] Identify the component(s) where tracking should be implemented
-   [ ] Import required dependencies (`useUsageTracker`, constants)
-   [ ] Initialize `useUsageTracker` hook in component
-   [ ] Map JSON properties to constant values
-   [ ] Implement tracking calls in appropriate lifecycle/event handlers
-   [ ] Handle dynamic properties based on component state
-   [ ] Handle conditional properties (omit null values)
-   [ ] Test the implementation (verify events in Amplitude dashboard or browser console)
-   [ ] Review code for consistency with best practices
-   [ ] Document any custom tracking logic with comments

---

## Troubleshooting

### Event Not Appearing in Amplitude

1. Check browser console for Amplitude initialization messages
2. Verify event name constant exists and matches exactly
3. Ensure required properties (`category`, `productGroup`) are included
4. Check that `UsageTrackerProvider` wraps your component tree

### Validation Errors

If you see validation errors in console:

1. Verify `category` is one of the valid `USAGE_TRACKING_EVENT_CATEGORIES`
2. Verify `productGroup` is one of the valid `USAGE_TRACKING_EVENT_PRODUCT_GROUPS`
3. Check that all property values use constants, not arbitrary strings

### Events Tracked Multiple Times

1. Check `useEffect` dependencies
2. Verify event handlers don't trigger unnecessarily
3. Ensure component isn't re-mounting unexpectedly

### Properties Not Appearing

1. Check that properties aren't `null` or `undefined`
2. Verify property names match expected format (camelCase)
3. Check that `appletDefinition.usageTracking` properties aren't being overridden

---

## Summary

To implement Amplitude tracking from `amplitude.input.json`:

1. **Prepare Constants**: Ensure all event names, categories, and property values are defined as constants
2. **Map Data**: Map JSON fields to the appropriate constants and property names
3. **Implement Tracking**: Use `useUsageTracker` hook and call `trackEvent()` with mapped properties
4. **Handle Dynamics**: Process conditional and multi-value properties based on runtime state
5. **Follow Patterns**: Use established patterns (applet opened/closed, action buttons, errors)
6. **Test**: Verify events appear correctly in Amplitude with expected properties

The key principle is to use constants everywhere, maintain consistency with existing tracking implementations, and leverage the framework's built-in features (like applet definition integration) to minimize code duplication.
