#pragma MIME application/x-tacticsogre-xlce-size26
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

// it's impractical to infer the exact type just from data
// edit next line manually if you think it is above type
using XlceEntry = XlceEntry_Common;
Xlce xlce @ 0x00 [[inline]];
