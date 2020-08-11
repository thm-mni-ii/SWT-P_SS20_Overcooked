# Building the Documentation
Follow these steps to build the documentation:

1. Open the command prompt inside the project folder
2. Run `git submodule update --init --recursive` to download all the needed files
3. Download and install [Doxygen](https://www.doxygen.nl/download.html)
4. Navigate the command prompt inside the `Docs` folder
5. Run `doxygen` (or just type the full path to the doxygen executable)
6. Doxygen should now start generating the documentation which will be available inside the `html` folder
7. When Doxygen is done, open `index.html` inside the `html` folder
8. You're done!
