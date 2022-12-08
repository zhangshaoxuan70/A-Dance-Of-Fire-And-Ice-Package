using System;

public class SaveStateScope : IDisposable
{
	private scnEditor editor;

	public SaveStateScope(scnEditor editor, bool clearRedo = false, bool onlySelectionChanged = false, bool skipSaving = false)
	{
		this.editor = editor;
		if (!skipSaving)
		{
			editor.SaveState(clearRedo, onlySelectionChanged);
		}
		editor.changingState++;
	}

	public void Dispose()
	{
		editor.changingState--;
	}
}
