#pragma MIME application/x-tacticsogre-xlce-sizeC4
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

struct XlceEntry_EntryUnit_Equipment {
	u16 itemId;  // 0 means use default equipment in this slot
	u8 dropRate;  // (dropRate/255)%
	padding[1];
};
struct XlceEntry_EntryUnit_Item {
	u16 itemId;
	padding[2];
};
struct XlceEntry_EntryUnit_Drop {
	u16 itemId;
	u8 amount;
	u8 dropRate;  // (dropRate/255)%
};
struct XlceEntry_EntryUnit {
	u16 nameId;
	u16 raceId;
	padding[2];
	u8 classId;
	padding[3];
	u8 positionX;
	u8 positionY;
	padding[62];
	XlceEntry_EntryUnit_Equipment equipments[7];
	XlceEntry_EntryUnit_Item items[4];
	XlceEntry_EntryUnit_Drop drops[4];
	padding[44];
	XlceEntry_EntryUnit_Drop escapedDrops[4];
	padding[2];
};

// it's impractical to infer the exact type just from data
// edit next line manually if you think it is above type
using XlceEntry = XlceEntry_Common;
Xlce xlce @ 0x00 [[inline]];
