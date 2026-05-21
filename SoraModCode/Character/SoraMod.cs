using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using SoraMod.SoraModCode.Cards.Basic;
using SoraMod.SoraModCode.Cards.Common.Attacks;
using SoraMod.SoraModCode.Cards.Common.Skills;
using SoraMod.SoraModCode.Cards.Rare.Skills;
using SoraMod.SoraModCode.Extensions;
using SoraMod.SoraModCode.Relics;

namespace SoraMod.SoraModCode.Character;

public class SoraMod : PlaceholderCharacterModel
{
    public const string CharacterId = "SoraMod";
    
    public static readonly Color Color = new("ffffff");

    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Neutral;
    public override int StartingHp => 20;
    public override bool ShouldAlwaysShowStarCounter => true;

    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<StrikeSoraMod>(),
        ModelDb.Card<StrikeSoraMod>(),
        ModelDb.Card<FocusSoraMod>(),
        ModelDb.Card<AerialSweepSoraMod>(),
        ModelDb.Card<SlidingDashSoraMod>(),
        ModelDb.Card<SlidingDashSoraMod>(),
        ModelDb.Card<VortexSoraMod>(),
        ModelDb.Card<VortexSoraMod>(),
        ModelDb.Card<BlizzardSoraMod>(),
        ModelDb.Card<BlizzardSoraMod>(),
    ];

    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<BronzeCrown>()
    ];

    public override CardPoolModel CardPool => ModelDb.CardPool<SoraModCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<SoraModRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<SoraModPotionPool>();
    
    public override Control CustomIcon
    {
        get
        {
            var icon = NodeFactory<Control>.CreateFromResource(CustomIconTexturePath);
            icon.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
            return icon;
        }
    }

    public override string CustomIconTexturePath => "sora_icon.png".CharacterUiPath();
    public override string CustomCharacterSelectIconPath => "sora_select.png".CharacterUiPath();
    public override string CustomCharacterSelectLockedIconPath => "char_select_char_name_locked.png".CharacterUiPath();
    public override string CustomMapMarkerPath => "sora_icon.png".CharacterUiPath();
    public override string CustomVisualPath => "res://SoraMod/animation/sora_combat_visuals.tscn";
}