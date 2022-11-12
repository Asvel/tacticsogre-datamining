#pragma MIME application/x-tacticsogre-txte
#pragma endian little

struct TxteOffset {
	u64 offset;
	char text[] @ offset;
};

struct TxteEntry {
	TxteOffset offsets[parent.entrySize / 8] [[inline]];
};

struct Txte {
	char signature[4];
	u32 count;
	u32 bodyStart;
	u32 entrySize;
	TxteEntry entries[count] @ bodyStart [[inline]];
};

Txte txte @ 0x00 [[inline]];
