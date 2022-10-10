# ~/build

## About

This is a set of MSBuild files meant to be treated modularly w.r.t. what kind of projects are in the Visual Studio SLN. This is accomplished by first defining a set of variables to help signal what kind of project is being built, and then those variables are used from within `.props` and `.targets` files from `components` which are imported with wildcards.

This is done to serve a few purposes:

- Defining package references in small files with a name hinting at a "theme" helps to document _why_ we have those dependencies.

- Defining project properties in small files that are shared across projects makes those properties be applied consistently.

- Defining package references in close proximity to related package references helps keep version numbers synchronized.

## How to Use

Include `build.props` from your `Directory.Build.props`, and include `build.targets` from your `Directory.Build.targets`.

Then you may:

- Add one or more `.props` files to the `components` folder and such files will automatically be imported as part of `Directory.Build.props`.

- Similarly, add one or more `.targets` files to the `components` folder and such files will automatically be imported as part of `Directory.Build.targets`.

- Remove any such files that are no longer a good fit.

Enjoy!
