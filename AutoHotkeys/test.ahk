Loop {
	Input, L, L1, {Esc}, L3
	if (%L% := "n"){
		Send, %L%
		ExitApp
	}
	Send, %L%
}
