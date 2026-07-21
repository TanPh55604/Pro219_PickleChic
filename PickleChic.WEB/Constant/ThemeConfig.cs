using MudBlazor;
using MudBlazor.Utilities;

namespace PickleChic.WEB.Constant
{
    public static class ThemeConfig
    {
        public static readonly MudTheme CustomerTheme = new()
        {
            PaletteLight = new PaletteLight
            {
                Primary = new MudColor("#d71920"),
                PrimaryContrastText = new MudColor("#ffffff"),

                Secondary = new MudColor("#111827"),
                SecondaryContrastText = new MudColor("#ffffff"),

                Tertiary = new MudColor("#f8fafc"),

                AppbarBackground = new MudColor("#ffffff"),
                AppbarText = new MudColor("#111827"),

                DrawerBackground = new MudColor("#ffffff"),
                DrawerText = new MudColor("#111827"),

                Background = new MudColor("#f8fafc"),
                Surface = new MudColor("#ffffff"),

                TextPrimary = new MudColor("#111827"),
                TextSecondary = new MudColor("#64748b"),

                LinesDefault = new MudColor("#e5e7eb"),
                TableLines = new MudColor("#e5e7eb"),
                Divider = new MudColor("#e5e7eb"),

                ActionDefault = new MudColor("#111827"),
                ActionDisabled = new MudColor("#9ca3af"),
                ActionDisabledBackground = new MudColor("#f1f5f9"),

                Success = new MudColor("#16a34a"),
                Warning = new MudColor("#f59e0b"),
                Error = new MudColor("#dc2626"),
                Info = new MudColor("#2563eb")
            },

            Typography = new Typography
            {
                Default = new DefaultTypography
                {
                    FontFamily = new[] { "Roboto", "Arial", "sans-serif" },
                    FontSize = "0.875rem",
                    FontWeight = "400",
                    LineHeight = "1.5"
                },

                Button = new ButtonTypography
                {
                    TextTransform = "none",
                    FontWeight = "800",
                    FontSize = "0.875rem"
                },

                H1 = new H1Typography
                {
                    FontWeight = "900",
                    LetterSpacing = "-0.04em"
                },

                H2 = new H2Typography
                {
                    FontWeight = "900",
                    LetterSpacing = "-0.035em"
                },

                H3 = new H3Typography
                {
                    FontWeight = "800",
                    LetterSpacing = "-0.03em"
                }
            },

            LayoutProperties = new LayoutProperties
            {
                DefaultBorderRadius = "14px"
            },

            Shadows = new Shadow
            {
                Elevation = new string[]
                {
                    "none",
                    "0 1px 2px rgba(15, 23, 42, 0.06)",
                    "0 4px 12px rgba(15, 23, 42, 0.08)",
                    "0 8px 24px rgba(15, 23, 42, 0.10)",
                    "0 12px 32px rgba(15, 23, 42, 0.12)",
                    "0 16px 40px rgba(15, 23, 42, 0.14)",
                    "0 20px 48px rgba(15, 23, 42, 0.16)",
                    "0 24px 56px rgba(15, 23, 42, 0.18)",
                    "0 28px 64px rgba(15, 23, 42, 0.20)",
                    "0 32px 72px rgba(15, 23, 42, 0.22)",
                    "0 36px 80px rgba(15, 23, 42, 0.24)",
                    "0 40px 88px rgba(15, 23, 42, 0.26)",
                    "0 44px 96px rgba(15, 23, 42, 0.28)",
                    "0 48px 104px rgba(15, 23, 42, 0.30)",
                    "0 52px 112px rgba(15, 23, 42, 0.32)",
                    "0 56px 120px rgba(15, 23, 42, 0.34)",
                    "0 60px 128px rgba(15, 23, 42, 0.36)",
                    "0 64px 136px rgba(15, 23, 42, 0.38)",
                    "0 68px 144px rgba(15, 23, 42, 0.40)",
                    "0 72px 152px rgba(15, 23, 42, 0.42)",
                    "0 76px 160px rgba(15, 23, 42, 0.44)",
                    "0 80px 168px rgba(15, 23, 42, 0.46)",
                    "0 84px 176px rgba(15, 23, 42, 0.48)",
                    "0 88px 184px rgba(15, 23, 42, 0.50)",
                    "0 92px 192px rgba(15, 23, 42, 0.52)",
                    "0 96px 200px rgba(15, 23, 42, 0.54)"
                }
            }
        };

        public static readonly MudTheme AdminTheme = new()
        {
            PaletteLight = new PaletteLight
            {
                Primary = new MudColor("#A05AFF"),
                PrimaryContrastText = new MudColor("#ffffff"),

                Secondary = new MudColor("#9E58FF"),
                SecondaryContrastText = new MudColor("#ffffff"),

                Tertiary = new MudColor("#f4f5f7"),

                AppbarBackground = new MudColor("#ffffff"),
                AppbarText = new MudColor("#1e293b"),

                DrawerBackground = new MudColor("#ffffff"),
                DrawerText = new MudColor("#6c7383"),

                Background = new MudColor("#f4f5f7"),
                Surface = new MudColor("#ffffff"),

                TextPrimary = new MudColor("#2c2e33"),
                TextSecondary = new MudColor("#6c7383"),

                LinesDefault = new MudColor("#e8eaed"),
                TableLines = new MudColor("#e8eaed"),
                Divider = new MudColor("#e8eaed"),

                ActionDefault = new MudColor("#6c7383"),
                ActionDisabled = new MudColor("#a0a4b0"),
                ActionDisabledBackground = new MudColor("#f4f5f7"),

                Success = new MudColor("#1BCFB4"),
                Warning = new MudColor("#f59e0b"),
                Error = new MudColor("#FE9496"),
                Info = new MudColor("#4BCBEB")
            },

            Typography = new Typography
            {
                Default = new DefaultTypography
                {
                    FontFamily = new[] { "Roboto", "Arial", "sans-serif" },
                    FontSize = "0.875rem",
                    FontWeight = "400",
                    LineHeight = "1.5"
                },

                Button = new ButtonTypography
                {
                    TextTransform = "none",
                    FontWeight = "700",
                    FontSize = "0.875rem"
                },

                H1 = new H1Typography
                {
                    FontWeight = "800",
                    LetterSpacing = "-0.03em"
                },

                H2 = new H2Typography
                {
                    FontWeight = "800",
                    LetterSpacing = "-0.025em"
                },

                H3 = new H3Typography
                {
                    FontWeight = "700",
                    LetterSpacing = "-0.02em"
                }
            },

            LayoutProperties = new LayoutProperties
            {
                DefaultBorderRadius = "12px"
            },

            Shadows = new Shadow
            {
                Elevation = new string[]
                {
                    "none",
                    "0 1px 2px rgba(15, 23, 42, 0.05)",
                    "0 4px 10px rgba(15, 23, 42, 0.07)",
                    "0 8px 20px rgba(15, 23, 42, 0.09)",
                    "0 12px 28px rgba(15, 23, 42, 0.11)",
                    "0 16px 36px rgba(15, 23, 42, 0.13)",
                    "0 20px 44px rgba(15, 23, 42, 0.15)",
                    "0 24px 52px rgba(15, 23, 42, 0.17)",
                    "0 28px 60px rgba(15, 23, 42, 0.19)",
                    "0 32px 68px rgba(15, 23, 42, 0.21)",
                    "0 36px 76px rgba(15, 23, 42, 0.23)",
                    "0 40px 84px rgba(15, 23, 42, 0.25)",
                    "0 44px 92px rgba(15, 23, 42, 0.27)",
                    "0 48px 100px rgba(15, 23, 42, 0.29)",
                    "0 52px 108px rgba(15, 23, 42, 0.31)",
                    "0 56px 116px rgba(15, 23, 42, 0.33)",
                    "0 60px 124px rgba(15, 23, 42, 0.35)",
                    "0 64px 132px rgba(15, 23, 42, 0.37)",
                    "0 68px 140px rgba(15, 23, 42, 0.39)",
                    "0 72px 148px rgba(15, 23, 42, 0.41)",
                    "0 76px 156px rgba(15, 23, 42, 0.43)",
                    "0 80px 164px rgba(15, 23, 42, 0.45)",
                    "0 84px 172px rgba(15, 23, 42, 0.47)",
                    "0 88px 180px rgba(15, 23, 42, 0.49)",
                    "0 92px 188px rgba(15, 23, 42, 0.51)",
                    "0 96px 196px rgba(15, 23, 42, 0.53)"
                }
            }
        };
    }
}