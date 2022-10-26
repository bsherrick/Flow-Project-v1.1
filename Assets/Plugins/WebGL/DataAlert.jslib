var Results = 
{
	ReportResults: function(str)
	{
		window.prompt("Copy to clipboard: Ctrl+C (or Cmd+C on Mac), Enter", Pointer_stringify(str));
	}
};

mergeInto(LibraryManager.library, Results);