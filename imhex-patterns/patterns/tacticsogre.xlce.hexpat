#pragma MIME application/x-tacticsogre-xlce
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
};

struct XlceEntry_Common32 {
	u32 data[parent.entrySize / 4] [[inline]];
};

struct XlceEntry_Common64 {
	u32 data[parent.entrySize / 8] [[inline]];
};

bitfield XlceEntry__GlobalFlagStorage {
	sizeType : 4;
	if (sizeType == 1) {
		bitPosition: 3;
		bytePosition: 9;
	}
	else if (sizeType == 2) {
		bitPosition: 1;
		bytePosition: 11;
	}
	else if (sizeType == 3) {
		bytePosition: 12;
	}
	else if (sizeType == 4) {
		bytePosition: 12;
	}
	else {
		bytePosition: 12;
	}
};
struct XlceEntry_GlobalFlag {
	XlceEntry__GlobalFlagStorage position [[inline]];
	u16 groupMask;
};

struct XlceEntry_MenuData26_ShopItem {
	// item available if: one of the requirements satisfaction AND that item exists
	// there are some canceled items still exist in this list
	// limited items (eg. Dragon Steak) are stored in other place
	u16 itemId;
	u16 requiredRecipe;
	u16 requiredCharacter;  // character is in barrack
	u16 increaseGlobalFlagWhenBuy;  // buy from shop
	u16 increaseGlobalFlagWhenSell;  // sell to shop
	u16 requiredGlobalFlag;
	padding[7];
	u8 requiredShopLevelOfNormalShops[14];  // 0 is not available
	padding[1];
	u8 requiredShopLevelOfDenebShop;  // 0 is not available
	u8 requiredShopLevelOfPotdShops[3];  // 0 is not available
};

struct XlceEntry_BattleData14_Class {
	padding[4];
	u16 growthHp;
	padding[4];
	u16 growthMp;
	padding[4];
	u16 growthStr;
	padding[4];
	u16 growthVit;
	padding[4];
	u16 growthDex;
	padding[4];
	u16 growthAgi;
	padding[4];
	u16 growthAvd;
	padding[4];
	u16 growthInt;
	padding[4];
	u16 growthMnd;
	padding[4];
	u16 growthRes;
	padding[2];
	u16 growthRandom;
	padding[144];
};

struct XlceEntry_BattleData17_Battlestage {
	padding[40];
	u16 entryunitId;
	padding[176];
	u16 entryunitIdSpecial;
	padding[10];
};

struct XlceEntry_BattleData23_Race {
	padding[2];
	u8 gender;
	padding[25];
	u8 growthHp;
	u8 growthMp;
	padding[1];
	u8 growthStr;
	padding[1];
	u8 growthVit;
	padding[1];
	u8 growthDex;
	padding[1];
	u8 growthAgi;
	padding[1];
	u8 growthAvd;
	padding[1];
	u8 growthInt;
	padding[1];
	u8 growthMnd;
	padding[1];
	u8 growthRes;
	padding[10];
	u8 classChangeType;
	u8 classChangeRace;
	padding[26];
};

struct XlceEntry_EntryUnit_Drop {
	u16 itemId;
	u8 amount;
	u8 dropRate; // (dropRate/255)%
};
struct XlceEntry_EntryUnit {
	u16 nameId;
	u16 raceId;
	padding[2];
	u8 classId;
	padding[3];
	u8 positionX;
	u8 positionY;
	padding[106];
	XlceEntry_EntryUnit_Drop drops[4];
	padding[62];
};

using XlceEntry = XlceEntry_Common;
Xlce xlce @ 0x00 [[inline]];
