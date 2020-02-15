# Grab MAC for Device - Dotnet Core 3.1

Donet Core 3.1 application that will grab the mac of the device that you are DIRECTLY connected to (e.g. an ethernet cable connected from your laptop to another laptop). The uses for this is mostly for DHCP reservations when you do not know the MAC of a client or the IP address to SSH in.

## Basic Usage

Build with your favorite IDE and you should be able to run it from your console in Linux and Windows! You will be asked for your main ethernet device on first startup, and it will write a file to remember this so you won't have to input it again. If for some reason this changes, go ahead and just delete that file and it will ask again.

## TODO

- [x] Better way to get main NIC
- [ ] Remove misc bugs that I'm sure are there
- [ ] Add a list of known NICs of local device so if it changes we can prompt again
- [ ] Easier to use interface for technology impared

If there are any questions, bugs, or feature requests please open an issue on gitlab/github! :dog:
