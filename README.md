# TeamStarter — Full-Stack Training Template

ده المشروع اللي هتبني عليه **نظام الحجوزات** طول شهر التدريب. المواصفات والخطة يوم بيوم في `TeamStarter-Guide.pdf`.
فيه **API واحد** بـ ASP.NET Core، و**اتنين frontend** (Vue و Angular) بيكلموا نفس الـ API.
فيه feature واحدة كاملة كمثال (**Tasks**) — اقرأها كويس، وكل feature جديدة تمشي على نفس الشكل.

```
TeamStarter/
├── backend/                     ASP.NET Core 10 Web API + EF Core + SQL Server
│   ├── src/TeamStarter.Api/
│   │   ├── Features/Tasks/      ← مثال كامل: Entity, DTOs, Service, Controller
│   │   ├── Data/                ← AppDbContext + Migrations
│   │   ├── Common/              ← Error handling موحّد (ProblemDetails)
│   │   └── Program.cs           ← تسجيل الـ services والـ pipeline
│   └── tests/TeamStarter.Api.Tests/   ← Integration tests (xUnit)
├── web-vue/                     Vue 3 + Vite + Pinia + Vue Router + Axios
│   └── src/features/tasks/      ← api + store + view
├── web-angular/                 Angular 21 + Signals + HttpClient
│   └── src/app/features/tasks/  ← api + store + page
└── docker-compose.yml           SQL Server للتطوير
```

---

## 1) المتطلبات

| الأداة | الإصدار |
|---|---|
| .NET SDK | 10 |
| Node.js | 22.12 أو أحدث |
| Docker Desktop | أي إصدار حديث |
| Editor | VS Code أو Visual Studio أو Rider |

## 2) التشغيل لأول مرة

**أ. شغّل الـ database**
```bash
docker compose up -d
```

**ب. شغّل الـ API** (الـ migrations بتتطبق لوحدها في الـ Development)
```bash
cd backend
dotnet tool restore
dotnet run --project src/TeamStarter.Api
```
افتح Swagger: http://localhost:5080/swagger

**ج. شغّل الـ frontend اللي بتشتغل بيه** (أو الاتنين)
```bash
cd web-vue
npm install
npm run dev          # http://localhost:5173
```
```bash
cd web-angular
npm install
npm start            # http://localhost:4200
```

> الـ frontend بيبعت أي request على `/api` للـ API على port 5080 عن طريق الـ proxy
> (`vite.config.ts` في Vue و `proxy.conf.json` في Angular)، فمش محتاج تغيّر أي URL.

**د. شغّل الـ tests**
```bash
cd backend
dotnet test
```

---

## 3) إزاي تضيف feature جديدة (Checklist)

مثال: `Categories` (أول feature هتبنيها في اليوم 2).

**Backend**
1. اعمل فولدر `Features/Categories/`.
2. `Category.cs` — الـ Entity + `IEntityTypeConfiguration` (أطوال الحقول، العلاقات).
3. ضيف `DbSet<Category>` في `AppDbContext`.
4. `CategoryDtos.cs` — الـ request/response records مع الـ validation attributes.
5. `CategoryService.cs` — الـ interface + الـ implementation (اللوجيك كله هنا، مش في الـ controller).
6. سجّل الـ service في `Program.cs`: `builder.Services.AddScoped<ICategoryService, CategoryService>();`
7. `CategoriesController.cs` — رفيع، بينادي الـ service بس.
8. اعمل migration:
   ```bash
   dotnet ef migrations add AddCategories -p src/TeamStarter.Api -o Data/Migrations
   ```
9. ضيف integration test في `tests/`.

**Frontend (Vue أو Angular)**
1. فولدر `features/categories/`.
2. `categories.api.ts` — الـ types (نفس شكل الـ DTOs) + الـ calls.
3. `categories.store.ts` — الـ state + loading + error.
4. الصفحة (`CategoriesView.vue` أو `categories.page.ts`) + الـ route.

---

## 4) القواعد المتفق عليها

- **الـ Controller رفيع**: مفيش لوجيك ولا `DbContext` جوه الـ controller.
- **مفيش Entity بيخرج من الـ API**: دايمًا DTO.
- **الأخطاء**: ارمي `NotFoundException` من الـ service وهي هتتحول لـ 404 لوحدها. الـ validation بيرجع 400 تلقائي.
- **القراءة بس؟** استخدم `AsNoTracking()` + `Select` لـ DTO.
- **كل feature في فولدر لوحدها** في الـ backend والـ frontend.
- **كل شغل يتسلّم كـ Pull Request** وأنا براجعه قبل الـ merge.

---

## 5) خريطة التدريب — هتلاقي `TODO (Week N)` في الكود

| الأسبوع | المطلوب في نظام الحجوزات |
|---|---|
| 1 (أيام 1-5) | الـ Entities كلها + Categories و Services و Providers و WorkingHours و TimeOff + search و filter و pagination |
| 2 (أيام 6-10) | Identity و JWT و refresh tokens و Roles + الـ availability + الحجز بكل قواعد العمل + الإلغاء والتقييم |
| 3 (أيام 11-15) | بوابة العميل (تصفح وحجز وحجوزاتي) + جدول الموظف + لوحة الأدمن و dashboard |
| 4 (أيام 16-20) | صور الخدمات + SignalR + background jobs + caching + tests + Docker و CI و deploy + الاختبار النهائي |

---

## 6) مشاكل شائعة

| المشكلة | الحل |
|---|---|
| الـ API مش قادر يوصل للـ DB | اتأكد إن `docker compose ps` شغال، واستنى ~20 ثانية أول مرة لحد ما SQL Server يقوم |
| `dotnet ef` مش موجود | `cd backend` ثم `dotnet tool restore` |
| الـ frontend بيرجع 404/502 على `/api` | الـ API لازم يكون شغال على port 5080 |
| `npm install` في Angular بيضرب error | الـ `.npmrc` فيه `legacy-peer-deps=true` — اتأكد إنه موجود |
