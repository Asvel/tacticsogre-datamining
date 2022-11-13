#pragma MIME application/x-tacticsogre-task
#pragma endian little

#include <std/mem.pat>
#include <tacticsogre/screenplay-instruction.pat>

struct TaskEntry {
	Instruction instructions[while(std::mem::read_signed($, 2) != -1)] [[inline]];
    padding[2];
};

struct TaskIndex {
	u32 offset;
	TaskEntry value @ offset [[inline]];
};

struct Task {
	char signature[4];
	u16 count;
	padding[2];
	u16 taskIds[count];
	padding[(count % 2) * 2];
	TaskIndex entries[count] [[inline]];
};

Task task @ 0x00 [[inline]];
