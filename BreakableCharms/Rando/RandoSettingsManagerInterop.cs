using RandoSettingsManager;
using RandoSettingsManager.SettingsManagement;
using RandoSettingsManager.SettingsManagement.Versioning;
using RandoSettingsManager.SettingsManagement.Versioning.Comparators;

namespace BreakableCharms.Randomizer;

public class RandoSettingsManagerInterop
{
    public static void Hook()
    {
        RandoSettingsManagerMod.Instance.RegisterConnection(new RandoSettingsProxy());
    }

    internal class RandoSettingsProxy : RandoSettingsProxy<RandoSettings, string>
    {
        public override string ModKey => BreakableCharms.Instance.GetName();

        public override VersioningPolicy<string> VersioningPolicy { get; }
            = new BackwardCompatiblityVersioningPolicy<string>(typeof(BreakableCharms).Assembly.GetName().Version.ToString(), new SemVerComparator());

        public override void ReceiveSettings(RandoSettings? settings)
        {
            if (settings != null)
            {
                BreakableCharms.globalSettings.RandomizeCharmLocations = settings.RandomizeCharmLocations;
                if (RandoMenu.SmallButton != null)
                {
                    RandoMenu.SmallButton.Text.color = BreakableCharms.globalSettings.RandomizeCharmLocations ? RandoMenu.OnColor : RandoMenu.OffColor;
                }
            }
            else
            {
                BreakableCharms.globalSettings.RandomizeCharmLocations = false;
                if (RandoMenu.SmallButton != null)
                {
                    RandoMenu.SmallButton.Text.color = BreakableCharms.globalSettings.RandomizeCharmLocations ? RandoMenu.OnColor : RandoMenu.OffColor;
                }
            }
        }

        public override bool TryProvideSettings(out RandoSettings? settings)
        {
            settings = new RandoSettings
            {
                RandomizeCharmLocations = BreakableCharms.globalSettings.RandomizeCharmLocations
            };
            return BreakableCharms.globalSettings.RandomizeCharmLocations;
        }
    }
}