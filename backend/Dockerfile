# השלב הראשון: בניית הפרויקט
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# העתקת קבצי הפרויקט (נניח שאת בתוך תיקיית ה-backend)
COPY . . 
RUN dotnet publish LearningPlatformApi/LearningPlatformApi.csproj -c Release -o out

# השלב השני: הרצת האפליקציה
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "LearningPlatformApi.dll"]