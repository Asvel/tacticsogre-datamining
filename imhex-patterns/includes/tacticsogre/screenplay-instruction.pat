#include <std/string.pat>

enum IT : s16 {
	/* 0x00 */ Noop = 0x00,
	/* 0x01 */ SetGlobalFlag,
	/* 0x02 */ SetLocalFlag,
	/* 0x03 */ ChangeClanApproval,  // aka. Chaos Frame
	/* 0x04 */ MoveToStrongpoint,  // strongpoint: place on the world map
	/* 0x05 */ ChangeWarFunds, // aka. GOTH
	/* 0x06 */ SkipDate,  // "few days later..."
	/* 0x07 */ ChangeLoyalty_SallyUnits_Lawful,  // sally unit means that unit participates current battle
	/* 0x08 */ ChangeLoyalty_SallyUnits_Chaotic,
	/* 0x09 */ ChangeLoyalty_SallyUnits_Neutral,
	/* 0x0a */ ChangeLoyalty_AllUnits_Lawful,  // all joined units
	/* 0x0b */ ChangeLoyalty_AllUnits_Chaotic,
	/* 0x0c */ ChangeLoyalty_AllUnits_Neutral,
	/* 0x0d */ SetProtagonistPersonality,  // personality: Lawful, Chaotic, Neutral
	/* 0x0e */ ResetGlobalFlagGroup,
	/* 0x0f */ SetUnitClan,
	/* 0x10 */ ChangeLoyalty_AllUnits_Clan,
	/* 0x11 */ SetUnitLoyalty,  // set to a absolute value, not change with a delta like above
	/* 0x12 */ MoveToStrongpointMini,  // move in minimap
	/* 0x13 */ SetProtagonistClan,
	/* 0x14 */ ObtainItem,
	/* 0x15 */ SetBattleUnitActionStrategy,  // change enemy AI
	/* 0x16 */ SuppressUnitRegularDeathScene,  // for using a specialized one during current battle
	/* 0x17 */ ChangeClearConditionMessage,  // and schedule a UI promotion
	/* 0x18 */ Noop18,
	/* 0x19 */ ChangeClearConditionUnit,  // and schedule a UI promotion
	/* 0x1a */ TransferUnitsAllegianceTo,  // transfer all units with same allegiance as protagonist
	/* 0x1b */ SetBattleUnitFaction,  // player / enemy / third-party
	/* 0x1c */ AddExtraGameOverConditionText,  // common/tables/battle_text.xlc[2840+x]
	/* 0x1d */ SetDungeonFlag,  // dungeon: PotD, Phorampa Wildwood, etc.
	/* 0x1e */ ResetDungeonFlags,
	/* 0x1f */ ChangeUnitClass,
	/* 0x20 */ ConsumeAndUnequipItem,
	/* 0x21 */ ObtainAppellation,  // aka. Order Title, they treat some other things (id > 100) as appellation too
	/* 0x22 */ IncreaseShopLevelTo,  // set GF_shop_level increase only
	/* 0x23 */ DisplayTargetMarkerOnBattleUnit,  // “Target” marker
	/* 0x24 */ ConsumeItemDuringBattle,
	/* 0x25 */ IncreaseUnionLevelTo,  // set GF_union_level increase only
	/* 0x26 */ IncreaseChariotTurnLimitTo,  // set GF_CHARIOT_turn_limit increase only
	/* 0x27 */ SetSystemFlag,  // cross-save flags
	/* 0x28 */ IncreaseGlobalFlagTo,  // set GFxxxx increase only
	/* 0x29 - 0x2f not used */

	/* 0x30 */ CompareGlobalFlag = 0x30,
	/* 0x31 */ CompareLocalFlag,
	/* 0x32 */ CompareCurrentStrongpoint,
	/* 0x33 */ CompareCurrentAtUnit,  // unit during Action Turn in battle
	/* 0x34 */ ComparePrimaryEnemyAmount,  // primary means exclude third-party units
	/* 0x35 */ ComparePrimaryEnemyAmountLessOrEqualExcept,  // usually except boss
	/* 0x36 */ ComparePrimaryEnemyAmountGreaterOrEqualExcept,  // usually except boss
	/* 0x37 */ CompareIfBattleUnitDead,
	/* 0x38 */ CompareBattleUnitCurrentHpLessOrEqual,  // percent%
	/* 0x39 */ CompareBattleUnitCurrentTpLessOrEqualFixed,
	/* 0x3a */ CompareBattleUnitCurrentTpLessOrEqual,  // percent%
	/* 0x3b */ CompareBattleUnitCurrentHpBetween,  // percent%
	/* 0x3c */ CompareIfBattleUnitAlive,
	/* 0x3d */ CompareBattleUnitCurrentHpGreaterOrEqual,  // percent%
	/* 0x3e */ CompareCurrentStrongpointMini,  // minimap node
	/* 0x3f */ CompareIfOnlyOneSallyUnit,  // for the Brigantys combat avoiding event
	/* 0x40 */ CompareIfProtagonistUnequipped,  // for the Brigantys combat avoiding event
	/* 0x41 */ CompareIfAnyPlayerUnitAtBattlestagePosition,  // compare to screenplays/global/2.xlc[x]
	/* 0x42 */ CompareUnitHeartCountGreaterOrEqual,
	/* 0x43 */ CompareUnitHeartCountLessOrEqual,
	/* 0x44 */ CompareIfUnitAtBattlestagePosition,  // compare to screenplays/global/2.xlc[x]
	/* 0x45 */ CompareDungeonFlag,  // dungeon: PotD, Phorampa Wildwood, etc.
	/* 0x46 */ CompareCurrentWeather,
	/* 0x47 */ CompareIfEntryUnitAtBattlestagePosition,
	/* 0x48 */ CompareClanApproval,  // aka. Chaos Frame
	/* 0x49 */ CompareUnitLoyaltyGreaterOrEqual,
	/* 0x4a */ CompareUnitLoyaltyLessOrEqual,
	/* 0x4b */ CompareIfBattleUnitReceivedAttack,  // for the two Lanselot duel event
	/* 0x4c */ CompareIfBattleUnitReceivedSupport,  // for the two Lanselot duel event
	/* 0x4d */ _Unknown4D,
	/* 0x4e */ CompareIfHasItem,
	/* 0x4f */ CompareCurrentShop,  // for test if deneb shop
	/* 0x50 */ CompareOverallDeceasedUnitsAmount,
	/* 0x51 */ CompareOverallIncapacitatedUnitsAmount,
	/* 0x52 */ CompareIfOverallChariotAndRetreatingNotUsed,
	/* 0x53 */ CompareIfUnitExistsInBarrack,  //  check if a character “playable” real time
	/* 0x54 */ _Unknown54,
	/* 0x55 */ CompareSystemFlag,  // cross-save flags
	/* 0x56 */ CompareSystemPlatform,  // Steam / Switch / PS4 / PS5
	/* 0x57 - 0x5f not used */

	/* 0x60 */ CompositeConditionAllTrue = 0x60,  // $1 and.. $n
	/* 0x61 */ CompositeCondition61,  // $0 and ($1 or $2) and $3
	/* 0x62 */ CompositeCondition62,  // $0 and $1 and ($2 or $3) and $4
	/* 0x63 */ CompositeCondition63,  // $0 and.. $2 and ($3 or $4)
	/* 0x64 */ CompositeCondition64,  // $0 and ($1 or $2)
	/* 0x65 */ CompositeCondition65,  // $0 or.. $5 or ($6 and $7)
	/* 0x66 */ CompositeCondition66,  // ($0 or $1) and ($2 or $3) and ($4 or $5) and ($6 or $7)
	/* 0x67 */ CompositeCondition67,  // $0 or $1
	/* 0x68 */ CompositeCondition68,  // $0 or.. $2
	/* 0x69 */ CompositeCondition69,  // $0 and.. $3 and ($4 or.. $6)
	/* 0x6a */ CompositeCondition6A,  // $0 and.. $5 and ($6 or $7)
	/* 0x6b */ CompositeCondition6B,  // $0 and.. $10 and ($11 or $12)
	/* 0x6c */ CompositeCondition6C,  // $0 and.. $8 and ($9 or $10)
	/* 0x6d */ CompositeCondition6D,  // $0 and.. $2 and ($3 or.. $6)
	/* 0x6e */ CompositeCondition6E,  // $0 and.. $4 and ($5 or $6)
	/* 0x6f */ CompositeCondition6F,  // $0 and.. $5 and ($6 or $7) and $8
	/* 0x70 */ CompositeCondition70,  // $0 and.. $6 and ($7 or $8)
	/* 0x71 */ CompositeCondition71,  // $0 and.. $3 and ($4 or.. $7)

	_ = -1,
};

fn getInstructionParameterSize(s16 type) {
    if (type == -1) return 0;
	return std::string::at("ADDCBEBBBBBBBBBDCDCBCECBCBBDBDADCBBCCBBDDAAAAAAAEECDCEEEEEEFEEDCCCEEEECDDEEDDCDCDDCDCECAAAAAAAAABBBBBBBBBBBBBBBBBB", type) - 'A';
};

struct Instruction {
	IT type;
	u8 parameters[getInstructionParameterSize(s16(type))] [[inline]];
};
