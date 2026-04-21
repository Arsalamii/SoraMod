using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using SoraMod.SoraModCode.Cards.Basic;
using SoraMod.SoraModCode.Cards.Common;
using SoraMod.SoraModCode.Cards.Rare;
using SoraMod.SoraModCode.Cards.Uncommon;
using SoraMod.SoraModCode.Extensions;
using SoraMod.SoraModCode.Relics;

namespace SoraMod.SoraModCode.Character;

public class SoraMod : PlaceholderCharacterModel
{
    public const string CharacterId = "SoraMod";
    
    public static readonly Color Color = new("ffffff");

    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Neutral;
    public override int StartingHp => 100;
    public override bool ShouldAlwaysShowStarCounter => true;

    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<StrikeSoraMod>(),
        ModelDb.Card<StrikeSoraMod>(),
        ModelDb.Card<StrikeSoraMod>(),
        ModelDb.Card<StrikeSoraMod>(),
        ModelDb.Card<GuardSoraMod>(),
        ModelDb.Card<GuardSoraMod>(),
        ModelDb.Card<GuardSoraMod>(),
        ModelDb.Card<GuardSoraMod>(),
        ModelDb.Card<FireSoraMod>(),
        ModelDb.Card<AeroSoraMod>(),
        ModelDb.Card<WisdomFormSoraMod>()
    ];

    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<CrownPendant>()
    ];

    public override CardPoolModel CardPool => ModelDb.CardPool<SoraModCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<SoraModRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<SoraModPotionPool>();

    /*  PlaceholderCharacterModel will utilize placeholder basegame assets for most of your character assets until you
        override all the other methods that define those assets.
        These are just some of the simplest assets, given some placeholders to differentiate your character with.
        You don't have to, but you're suggested to rename these images. */
    public override Control CustomIcon
    {
        get
        {
            var icon = NodeFactory<Control>.CreateFromResource(CustomIconTexturePath);
            icon.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
            return icon;
        }
    }

    public override string CustomIconTexturePath => "character_icon_char_name.png".CharacterUiPath();
    public override string CustomCharacterSelectIconPath => "char_select_char_name.png".CharacterUiPath();
    public override string CustomCharacterSelectLockedIconPath => "char_select_char_name_locked.png".CharacterUiPath();
    public override string CustomMapMarkerPath => "map_marker_char_name.png".CharacterUiPath();
}