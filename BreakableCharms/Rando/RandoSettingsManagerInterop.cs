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

        public override VersioningPolicy<string> VersioningPolicy => new StrictModVersioningPolicy(BreakableCharms.Instance);

        public override void ReceiveSettings(RandoSettings? settings)
        {
            BreakableCharms.globalSettings.RandomizeCharmLocations = settings is { RandomizeCharmLocations: true };
            RandoMenu.SetButtonColor(BreakableCharms.globalSettings.RandomizeCharmLocations);
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