#Git Remote Change
We created Git Remote Change to easily bulk-convert repos between HTTPS and SSH. Running this executable will quickly update all repos for the desired protocol.

## Customizing GitRemoteChange for your Needs
1. Open `Program.cs`.
1. Change constants `ACCOUNT`, `COLLECTION`, and `PROJECT` to your values.

## Usage
1. Open GitRemoteChange.sln.
1. Build the solution.
1. Execute `GitRemoteChange.exe` in _bin\debug_ folder. The help will tell you what parameters can be used.

## Updating code
When you make changes to the code, please update the version in `PrintHelp` as well as CHANGELOG.md.

## Attribution
Thanks to [Brian Drupieski](https://github.com/bdrupieski) for the original idea and code!