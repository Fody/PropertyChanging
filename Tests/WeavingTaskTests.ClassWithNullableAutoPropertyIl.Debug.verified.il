.method public hidebysig specialname 
	instance void set_IsFlag (
		valuetype [mscorlib]System.Nullable`1<bool> 'value'
	) cil managed 
{
	.custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
		01 00 00 00
	)
	// Header size: 12
	// Code size: 36 (0x24)
	.maxstack 2

	IL_0000: ldarg.0
	IL_0001: ldfld valuetype [mscorlib]System.Nullable`1<bool> ClassWithNullableAutoProperty::'<IsFlag>k__BackingField'
	IL_0006: ldarg.1
	IL_0007: call bool [mscorlib]System.Nullable::Equals<bool>(valuetype [mscorlib]System.Nullable`1<!!0>, valuetype [mscorlib]System.Nullable`1<!!0>)
	IL_000c: brfalse.s IL_000f

	IL_000e: ret

	IL_000f: nop
	IL_0010: ldarg.0
	IL_0011: ldstr "IsFlag"
	IL_0016: callvirt instance void ClassWithNullableAutoProperty::OnPropertyChanging(string)
	IL_001b: ldarg.0
	IL_001c: ldarg.1
	IL_001d: stfld valuetype [mscorlib]System.Nullable`1<bool> ClassWithNullableAutoProperty::'<IsFlag>k__BackingField'
	IL_0022: nop
	IL_0023: ret
} // end of method ClassWithNullableAutoProperty::set_IsFlag
