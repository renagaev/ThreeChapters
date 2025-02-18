FROM mcr.microsoft.com/dotnet/sdk:8.0 as backend

WORKDIR src

COPY src/*.sln ./
COPY src/**/*.csproj ./

RUN for f in *.csproj; do \
        filename=$(basename $f) && \
        dirname=${filename%.*} && \
        mkdir $dirname && \
        mv $filename ./$dirname/; \
    done
RUN dotnet restore ThreeChapters.API/ThreeChapters.API.csproj

COPY src ./
RUN dotnet publish ThreeChapters.API/ThreeChapters.API.csproj --output ./publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 as final 
WORKDIR app 
COPY --from=backend src/publish .

EXPOSE 80
ENV TZ=Europe/Moscow
ENV ASPNETCORE_URLS http://0.0.0.0:80
ENTRYPOINT ["dotnet", "ThreeChapters.API.dll"]