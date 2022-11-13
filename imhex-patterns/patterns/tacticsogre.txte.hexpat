#pragma MIME application/x-tacticsogre-txte
#pragma endian little

struct TxteEntry {
	u64 keyOffset;
	u64 valueOffset;
	u64 unknown1Offset;
	u64 unknown2Offset;

	char key[] @ keyOffset;
	char value[] @ valueOffset;
	char unknown1[] @ unknown1Offset;
	char unknown2[] @ unknown2Offset;
};

struct Txte {
	char signature[4];
	u32 count;
	u32 bodyStart;
	u32 entrySize;
	TxteEntry entries[count] @ bodyStart [[inline]];
};

Txte txte @ 0x00 [[inline]];
