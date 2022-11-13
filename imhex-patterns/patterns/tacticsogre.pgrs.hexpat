#pragma MIME application/x-tacticsogre-pgrs
#pragma endian little

enum PNT : s8 {  // FUN_088cc768
    /* 0x00 */ Unconditional,
    /* 0x01 */ StoreAndComparePlayerSelection,  // yes, a compare operation with side effect
    /* 0x02 */ CompareGlobalFlag,
    /* 0x03 */ CompareLocalFlag,
    /* 0x04 */ CheckInvk,
    /* 0x05 */ CompareCurrentReplayBranch,  // replay past scenario in Warren Report
    /* 0x06 */ CompareIfUnitExistsInBarrack,  // check if a character “playable” real time
};

enum PMT : s8 {  // FUN_088cdcd8
    /* 0x00 */ None,
    /* 0x01 */ AddGuest,
    /* 0x02 */ AddUnit,
    /* 0x03 */ AddGeneralUnit,
    /* 0x04 */ RemoveGuest,
    /* 0x05 */ SystemMessage,
    /* 0x06 */ PromptDataSavingWithMessage,
    /* 0x07 */ _Unknown07,
    /* 0x08 */ NameInput,
    /* 0x09 */ SystemMessageDuringBattle,
    /* 0x0a */ SaveFlagsIntoWorldAnchor,  // W.O.R.L.D. AnchorPoint
    /* 0x0b */ ChangeChapter,
    /* 0x0c */ _Unknown0c,
    /* 0x0d */ AddGuestWithMessage,
    /* 0x0e */ RemoveDeadUnit,
};

bitfield PgrsFlags {
    wrTerminate : 1;
    padding : 6;
    isScenario : 1;
};

struct PgrsEntry {
    /* 0x00 */ u16 stageId;
	/* 0x02 */ u16 mapId;
	/* 0x04 */ u16 sceneId;
	/* 0x06 */ u16 invkId;
	/* 0x08 */ u16 taskId;
	/* 0x0a */ u16 nextSceneId;
	/* 0x0c */ u16 nextTaskId;
	/* 0x0e */ u16 menuTaskDataId;
	/* 0x10 */ u16 battlestageId;
	/* 0x12 */ bool battleTeamFormation;
	/* 0x13 */ u8 encounterRate;
	/* 0x14 */ u16 eventEntryId;
	/* 0x16 */ u16 nextingCompareValue;
	/* 0x18 */ u8 nextingCompareOperator;
	/* 0x19 */ PNT nextingType;
	/* 0x1a */ u16 nextingFlagId;
	/* 0x1c */ u8 dayNightVariation;
	/* 0x1d */ u8 weatherDefault;
	/* 0x1e */ PgrsFlags flags [[inline]];
	/* 0x1f */ PMT menuTaskType;
	/* 0x20 */ u16 menuTaskParam1;
	/* 0x22 */ u16 menuTaskParam2;
	/* 0x24 */ u8 nextOtherId;
	/* 0x25 */ u8 seOfCircumstance;
	/* 0x26 */ u8 bgmForEvent;
	/* 0x27 */ u8 bgmForBattle;
	/* 0x28 */ u8 bgmControlOpening;
	/* 0x29 */ u8 bgmControlEnding;
	/* 0x2a */ u8 _unknown1;
	/* 0x2b */ u8 _unknown2;
};

struct Pgrs {
	char signature[4];
	u16 _unknown;
	u16 count;
	PgrsEntry entries[count] [[inline]];
};

Pgrs pgrs @ 0x00 [[inline]];
