# FinanceApp

FinanceApp adalah aplikasi manajemen keuangan pribadi berbasis web untuk mencatat pemasukan/pengeluaran, mengelola rekening (wallet), dan melihat ringkasan dashboard.

## Fitur Utama

- Autentikasi dengan JWT + refresh token berbasis cookie `HttpOnly`.
- Verifikasi email saat registrasi.
- Lupa password + reset password via email.
- Manajemen wallet: tambah, lihat, ubah, hapus.
- Manajemen transaksi: tambah, lihat, ubah, hapus (soft delete).
- Dashboard ringkasan:
  - Total saldo seluruh wallet
  - Total pemasukan bulan ini
  - Total pengeluaran bulan ini
  - 5 transaksi terbaru
  - 3 wallet teratas
  - Tren aset 30 hari
- Proteksi rate limiting pada endpoint auth dan wallet.

## Arsitektur

Project terdiri dari 2 aplikasi:

- `frontend/`: Nuxt 4 + Vue 3 + Pinia + Tailwind + shadcn-nuxt
- `backend/`: ASP.NET Core Web API + EF Core + PostgreSQL

Alur autentikasi:

1. User login via `POST /api/auth/login`.
2. Backend set cookie `accessToken` dan `refreshToken` (HttpOnly).
3. Frontend request API dengan `credentials: include`.
4. Jika `401`, plugin frontend otomatis mencoba `POST /api/auth/refresh`.
5. Jika refresh gagal, user diarahkan kembali ke halaman login.

## Struktur Folder Ringkas

```text
FinanceApp/
|- backend/
|  |- FinanceApp.API/
|  |- FinanceApp.Test/
|- frontend/
|  |- app/
|  |- public/
|- run.cmd
```

## Prasyarat

- .NET SDK 9+
- Node.js 20+
- pnpm
- PostgreSQL

## Konfigurasi Environment

### Backend (`backend/FinanceApp.API/.env`)

Backend membaca file `.env` otomatis jika tersedia.

Contoh nilai yang perlu disediakan:

```env
# Database
ConnectionStrings__DefaultConnection=Host=localhost;Port=5432;Database=financeapp;Username=postgres;Password=postgres

# JWT
Jwt__Key=GantiDenganSecretKeyPanjangMinimal32Karakter
Jwt__Issuer=FinanceApp.API
Jwt__Audience=FinanceApp.Web
Jwt__ExpireMinutes=60

# SMTP
Smtp__Host=smtp.gmail.com
Smtp__Port=587
Smtp__Email=youremail@example.com
Smtp__Password=your-app-password

# URL frontend untuk email action
App__VerifyEmailUrl=http://localhost:3000/verify-email
App__ResetPasswordUrl=http://localhost:3000/reset-password

# CORS (pisahkan array dengan ;)
Cors__FrontendOrigins__0=http://localhost:3000
Cors__DevOrigins__0=http://localhost:3000
```

Catatan:

- Di environment variable .NET, gunakan `__` untuk nested key (contoh `Jwt__Key`).
- Untuk production, pastikan `Cors__FrontendOrigins` diisi domain frontend yang valid.

### Frontend (`frontend/.env`)

```env
NUXT_PUBLIC_API_BASE=http://localhost:5000
```

Sesuaikan port backend dengan konfigurasi lokal Anda.

## Menjalankan Aplikasi

### 1. Frontend

```powershell
cd frontend
pnpm install
pnpm dev
```

### 2. Backend

```powershell
cd backend
dotnet restore FinanceApp.slnx
dotnet run --project FinanceApp.API/FinanceApp.API.csproj
```

### Opsi via root script

Di root project tersedia `run.cmd`:

```powershell
.\run.cmd frontend
.\run.cmd backend
.\run.cmd backend:test
```

## Migrasi Database

Jalankan dari folder `backend/FinanceApp.API`:

```powershell
dotnet ef database update
```

Jika tool EF belum ada:

```powershell
dotnet tool install --global dotnet-ef
```

## Endpoint API Inti

Base URL: `http://localhost:<port-backend>/api`

### Auth

- `POST /auth/register`
- `POST /auth/login`
- `GET /auth/me` (butuh login)
- `POST /auth/verify-email`
- `POST /auth/forgot-password`
- `POST /auth/reset-password`
- `POST /auth/refresh`
- `POST /auth/logout`

### Wallet (butuh login)

- `GET /wallet`
- `POST /wallet`
- `GET /wallet/{id}`
- `PATCH /wallet/{id}`
- `DELETE /wallet/{id}`

### Transaction (butuh login)

- `GET /transaction`
- `POST /transaction/{walletId}`
- `GET /transaction/{id}`
- `PATCH /transaction/{id}`
- `DELETE /transaction/{id}`

### Dashboard (butuh login)

- `GET /dashboard`

### Health & Docs

- `GET /health`
- `GET /swagger`

## Tipe Transaksi

Enum transaksi di backend:

- `0` = `Income` (Pemasukan)
- `1` = `Expense` (Pengeluaran)

Frontend saat ini juga menggunakan mapping angka tersebut.

## Keamanan yang Sudah Ada

- Password di-hash sebelum disimpan.
- JWT access token + rotasi refresh token.
- Revoke token saat logout/reset password.
- Lockout login sementara setelah beberapa kali gagal.
- Rate limiting per IP/user untuk endpoint sensitif.

## Testing

```powershell
cd backend
dotnet test FinanceApp.slnx
```

## Troubleshooting Cepat

- `401 Unauthorized` terus menerus:
  - Pastikan `NUXT_PUBLIC_API_BASE` benar.
  - Pastikan browser mengizinkan cookie untuk domain backend.
- CORS error:
  - Periksa `Cors__DevOrigins__0` / `Cors__FrontendOrigins__0`.
- Email verifikasi/reset tidak terkirim:
  - Periksa konfigurasi `Smtp__*`.
- Gagal konek database:
  - Periksa `ConnectionStrings__DefaultConnection`.

## Catatan Pengembangan

- API menggunakan cookie-based auth (`credentials: include` wajib di frontend).
- Soft delete transaksi menggunakan `DeletedAt`.
- Perubahan transaksi akan menyesuaikan saldo wallet terkait.
