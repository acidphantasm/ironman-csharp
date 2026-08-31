namespace IronManServer.Helpers;

using Models.Enums;
using IronManServer.Web.Models;

public static class WebProfileHelper
{
    public static readonly IReadOnlyList<ProfileInfo> Profiles =
    [
        new(
            ProfileType.Standard,
            "Standard",
            "The beginning",
            "I",
            "standard",
            "A challenging Ironman experience with meaningful restrictions and consequences without completely rebuilding how Tarkov is played.",
            2,
            "Moderate",
            [
                new(
                "Bonuses",
                [
                    new("Level Experience Gain", "1×"),
                    new("Skill Experience Gain", "1×"),
                    new("Container Gift", "5×3 & 6×5 Containers Gifted @ Start"),
                ]),
                new(
                    "Economy",
                    [
                        new("Insurance Cost", "1×"),
                        new("Base Repair Cost", "1×"),
                        new("Therapist Healing Cost", "2×"),
                        new("Flea Market", "Disabled"),
                        new("Trader Purchases", "Limited")
                    ]),

                new(
                    "Death",
                    [
                        new("Loose Items", "Items valued over 100k lost"),
                        new("Items in Containers", "Protected"),
                        new("Container Items", "Protected"),
                        new("Protected Currency Limit", "1m ₽ / 2k $ / 1k € / 50 GP"),
                        new("Skill Loss", "Current Progress Lost"),
                        new("Experience Loss", "Current Progress Lost"),
                    ]),

                new(
                    "Progression",
                    [
                        new("Starting Equipment", "Minimal"),
                        new("Stash Size", "10×30"),
                        new("Death Downgrade", "No Downgrade"),
                        new("Scav Runs", "No Changes")
                    ])
            ]),

        new(
            ProfileType.Ultimate,
            "Ultimate",
            "The serious challenge",
            "II",
            "ultimate",
            "A significantly harsher experience where survival and progression become increasingly important. Death can cost more than your equipment.",
            4,
            "High",
            [
                new(
                "Bonuses",
                [
                    new("Level Experience Gain", "1.25×", RuleChangeType.Changed),
                    new("Skill Experience Gain", "1.25×", RuleChangeType.Changed),
                    new("Container Gift II", "5×5 & 10×10 Containers Gifted @ Lv15", RuleChangeType.Added),
                ]),
                new(
                    "Economy",
                    [
                        new("Insurance Cost", "2×", RuleChangeType.Changed),
                        new("Base Repair Cost", "2×", RuleChangeType.Changed),
                        new("Therapist Healing Cost", "5×", RuleChangeType.Changed)
                    ]),

                new(
                    "Loot",
                    [
                        new("Corpse Loot", ">50% Damage Dealt Only", RuleChangeType.Added)
                    ]),

                new(
                    "Death",
                    [
                        new("Loose Items", "All items lost", RuleChangeType.Changed),
                        new("Protected Currency Limit", "500k ₽ / 1k $ / 500 € / 25 GP", RuleChangeType.Changed),
                        new("Skill Level Loss", "Up to 1 level", RuleChangeType.Added),
                        new("Experience Level Loss", "Up to 1 level", RuleChangeType.Added)
                    ]),

                new(
                    "Progression",
                    [
                        new("Stash Size", "10×25", RuleChangeType.Changed),
                        new("Death Downgrade", "Optionally downgrade to Standard", RuleChangeType.Changed),
                        new("Scav Runs", "Items are automatically sold at 75% value", RuleChangeType.Changed)
                    ])
            ]),

        new(
            ProfileType.Hardcore,
            "Hardcore",
            "No room for mistakes",
            "III",
            "hardcore",
            "The ultimate Ironman experience. Every raid matters, every death hurts, and a single mistake can undo hours of progression.",
            5,
            "Extreme",
            [
                new(
                "Bonuses",
                [
                    new("Level Experience Gain", "2×", RuleChangeType.Changed),
                    new("Skill Experience Gain", "2×", RuleChangeType.Changed),
                    new("Container Gift II", "6×5 & 9×10 Containers Gifted @ Lv15", RuleChangeType.Changed),
                    new("Container Gift III", "6×8 & 12×12 Containers Gifted @ Lv25", RuleChangeType.Added),
                ]),
                new(
                    "Economy",
                    [
                        new("Insurance Cost", "5×", RuleChangeType.Changed),
                        new("Base Repair Cost", "5×", RuleChangeType.Changed),
                        new("Therapist Healing Cost", "No Therapist Healing", RuleChangeType.Changed)
                    ]),

                new(
                    "Loot",
                    [
                        new("Corpse Loot", ">50% Damage Dealt Only", RuleChangeType.Changed)
                    ]),

                new(
                    "Death",
                    [
                        new("Loose Items", "All items lost", RuleChangeType.Changed),
                        new("Protected Currency Limit", "100k ₽", RuleChangeType.Changed),
                        new("Skill Level Loss", "Up to 3 levels", RuleChangeType.Changed),
                        new("Experience Level Loss", "Up to 3 levels", RuleChangeType.Changed)
                    ]),

                new(
                    "Progression",
                    [
                        new("Stash Size", "10×20", RuleChangeType.Changed),
                        new("Death Downgrade", "Optionally downgrade to Ultimate", RuleChangeType.Changed),
                        new("Scav Runs", "Items are automatically sold at 50% value", RuleChangeType.Changed)
                    ])
            ])
    ];

    public static ProfileInfo GetInfo(ProfileType type)
    {
        return Profiles.FirstOrDefault(p => p.Type == type) ?? throw new ArgumentOutOfRangeException(nameof(type), type, "Unknown profile type.");
    }

    public static IReadOnlyList<RuleGroup> GetRules(ProfileType type)
    {
        var standard = GetInfo(ProfileType.Standard);

        if (type == ProfileType.Standard)
        {
            return standard.RuleGroups
                .Select(group => new RuleGroup(group.Name, group.Rules.Select(rule => new Rule(rule.Name, rule.Value)).ToList()))
                .ToList();
        }

        var selected = GetInfo(type);

        var result = new List<RuleGroup>();

        foreach (var standardGroup in standard.RuleGroups)
        {
            var selectedGroup = selected.RuleGroups.FirstOrDefault(group => group.Name.Equals(standardGroup.Name, StringComparison.OrdinalIgnoreCase));

            var rules = new List<Rule>();

            foreach (var standardRule in standardGroup.Rules)
            {
                var selectedRule = selectedGroup?.Rules.FirstOrDefault(rule => rule.Name.Equals(standardRule.Name, StringComparison.OrdinalIgnoreCase));

                if (selectedRule is null)
                {
                    rules.Add(new Rule(standardRule.Name, standardRule.Value));
                    continue;
                }

                rules.Add(selectedRule);
            }

            if (selectedGroup is not null)
            {
                foreach (var selectedRule in selectedGroup.Rules)
                {
                    var existsInStandard = standardGroup.Rules.Any(rule => rule.Name.Equals(selectedRule.Name, StringComparison.OrdinalIgnoreCase));

                    if (!existsInStandard)
                        rules.Add(selectedRule);
                }
            }

            result.Add(new RuleGroup(standardGroup.Name, rules));
        }

        foreach (var selectedGroup in selected.RuleGroups)
        {
            var existsInStandard = standard.RuleGroups.Any(group => group.Name.Equals(selectedGroup.Name, StringComparison.OrdinalIgnoreCase));

            if (!existsInStandard)
                result.Add(new RuleGroup(selectedGroup.Name, selectedGroup.Rules.ToList()));
        }

        return result;
    }
}