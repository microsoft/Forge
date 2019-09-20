
# Contributing

Welcome, and thank you for your interest in contributing to Forge! This project welcomes contributions and suggestions.
Most contributions require you to agree to a Contributor License Agreement (CLA) declaring that you have the right to,
and actually do, grant us the rights to use your contribution. For details, visit https://cla.microsoft.com.

When you submit a pull request, a CLA-bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., label, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

---
## How to submit a PullRequest to Forge GitHub

### Quick note about branch names and the versioning convention used:
* When creating a new branch, prefix the branch name with "feature/".
* Ex) git branch feature/ForgeActionAttribute
* A new Forge DevOps build is kicked off each time either the master branch or a "feature/" branch gets pushed to GitHub.
* GitVersion is used to handle the versioning. It's set up for the standard SemVer, Major.Minor.Patch-<tags>.
* feature/ branches have the <alpha> tag appended to ensure they cannot be pushed as official Nuget updates.
* master branch updates do not have tags appended.

### Git commands
(Use cmd window for all commands)

```cmd
echo Set up new git workspace…
mkdir works-forge
cd works-forge
git init

echo You may be prompted for GitHub sign-in info…
git remote add origin https://github.com/microsoft/Forge.git
Git pull
git checkout master

echo Make changes on new branch and push…
git branch feature/<branch name>
git checkout feature/<branch name>
git commit -a -m "Adding new feature X."
git push --set-upstream origin feature/<branch name>
```

### Creating a PullRequest
After pushing your "feature/" branch upstream, you can view and create a PullRequest on the GitHub page for your branch.
https://github.com/microsoft/Forge