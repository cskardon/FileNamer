# FileNamer

Just goes through a folder (and subfolders) to allow you to replace a set of characters in a filename with another set.

## Usage

Remember to use `--safe-mode` to make sure it will rename to what you want it to. **Also** if you're going to end up renaming
a lot of files - add something like `> output.txt` to get the output into a file that can be parsed in your favourite editor.

```
.\FileNamer.exe --dir "D:\temp" --oldchars ┬ú --newchars GBP --sub --remove --safe-mode
```

## Help
```
FileNamer.exe --d <DIRECTORY> --oc <OLD_CHARACTERS> --nc <NEW_CHARACTERS> [--s] [--r] [--safe-mode]

    --d        : The root directory to rename files from.
    --oc       : The Old Characters to replace.
    --nc       : The New Characters to replace the Old Characters with.
    --s        : [OPTIONAL] If defined, FileNamer will parse the sub folders as well.
    --r        : [OPTIONAL] If defined, FileNamer will remove the original file.
    --safe-mode: [OPTIONAL] If defined, FileNamer won't actually do anything, just show you what it would do.
```

## Why?

This is a .NET 4.6.2 project - not core, so that it can just work on a Windows 10 machine.
It also copes with filename lengths > 265 (Windows' limit of `MAX_LENGTH`) and is an experiment to show this working.