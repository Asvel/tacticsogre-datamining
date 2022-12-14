#pragma MIME application/x-tacticsogre-xlce-sizeB4
#pragma endian little

using XlceEntry;

struct Xlce {
	char signature[4];
	u32 count;
	u32 bodyStart;
	u32 entrySize;
	XlceEntry entries[count] @ bodyStart [[inline]];
};

struct XlceEntry_Common {
	u8 data[parent.entrySize] [[inline]];
	// u32 data[parent.entrySize / 4] [[inline]];
};

struct XlceEntry_BattleData16_Action_Sub {
	u8 type1;
	padding[7];
	u8 type2;  // type1==1 && type2==3 is regular attack
	u8 attackTypeId;  // refer to BattlData19[attackTypeId], BattlData19[attackTypeId + 1]
	u16 actionAtk;
	padding[7];
	u8 actionElementType;
	padding[4];
	u16 actionElementValue;
};
struct XlceEntry_BattleData16_Action {  // magic, finishing, class action skill
	padding[2];
	u8 actionCategory;
	padding[5];
	u8 mpCost;
	padding[5];
	u8 range;  // count unit's space, eg. 7 means 6 extra range
	u8 rangeExclude;
	padding[26];
	XlceEntry_BattleData16_Action_Sub actionSubs[3];
	padding[28];
	u16 nameId;
	padding[30];
};

// it's impractical to infer the exact type just from data
// edit next line manually if you think it is above type
using XlceEntry = XlceEntry_Common;
Xlce xlce @ 0x00 [[inline]];
