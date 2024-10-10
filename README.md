# ChatClaude for PowerToys Run

### How to install

If you simply want to install the plugin to get up and running quickly, i suggest downloading the [precompiled binaries](https://github.com/mba/ChatClaudePowerToys/releases) from the Release section.
The installation process goes as follows:

1. Locate your PowerToys installation (eg. `C:\Program Files\PowerToys`)
1. Navigate to `\RunPlugins`
1. Unpack the downloaded binaries

### Compiling the plugin

1. Clone the PowerToys repository to your local disk using the command `git clone https://github.com/microsoft/PowerToys.git`
1. Navigate to the PowerToys directory using `cd PowerToys`
1. Initialize and update submodules with the command `git submodule update --init --recursive`
1. Fork the ChatGPTPowerToys repository on GitHub
1. Clone the fork of ChatGPTPowerToys into the local PowerToys repository by running `git clone https://github.com/ferraridavide/ChatGPTPowerToys.git` in the `PowerToys\src\modules\launcher\Plugins` directory
1. In Visual Studio, add the local clone of ChatGPTPowerToys as an existing project to the PowerToys's Plugins folder (`modules\launcher\Plugins`)
1. Compile

For **PWA support**, see [#13](https://github.com/ferraridavide/ChatGPTPowerToys/issues/13)
