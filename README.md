# Wiki Page Stats Generator

[![NuGet](https://img.shields.io/nuget/v/WikiPageStatsGenerator)](https://www.nuget.org/packages/WikiPageStatsGenerator) [![codecov](https://codecov.io/gh/ViMaSter/wiki-page-stats-generator/branch/main/graph/badge.svg?token=T7ESI3L6ZN)](https://codecov.io/gh/ViMaSter/wiki-page-stats-generator) [![Build, Release, Publish](https://github.com/ViMaSter/wiki-page-stats-generator/actions/workflows/build-release-publish.yml/badge.svg)](https://github.com/ViMaSter/wiki-page-stats-generator/actions/workflows/build-release-publish.yml)

---

Wiki Page Stats Generator is a dotnet app that generates Azure Wiki-friendly markdown files about the most viewed pages of a specific Azure DevOps Wiki.  
## Usage

![image](https://user-images.githubusercontent.com/1689033/229325689-b2d8a4bb-c6b2-4b0e-8fce-291de88da53c.png)
Based on this URL use the following parameters:
```bash
dotnet tool install --global WikiPageStatsGenerator

# obtain an Azure PAT:
# https://learn.microsoft.com/en-us/azure/devops/organizations/accounts/use-personal-access-tokens-to-authenticate?view=azure-devops&tabs=Windows#create-a-pat 

# this assumes you want to output the markdown files to `/wiki`
WikiPageStatsGenerator \
  --azure-pat <azure-pat> \
  --organization <organization> #vimaster \
  --project <project> #newtendoland \
  --wiki-identifier <wiki> #newtendoland-wiki \
  --output-path /wiki
```

This will generate a file at `/wiki/Most-Viewed-Pages.md` with the following content:
![image](https://user-images.githubusercontent.com/1689033/229325776-9c4abc8a-71c3-4f61-8267-e998b29bf7c7.png)

## License

[GNU General Public License v3.0](https://choosealicense.com/licenses/gpl-3.0/)
