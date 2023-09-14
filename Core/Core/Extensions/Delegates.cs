namespace ModularSystem.Core;

public delegate void RefAction<Input>(ref Input input);
public delegate void OutAction<Input>(out Input input);