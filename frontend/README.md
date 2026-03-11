# Fertile Notify Frontend

The frontend web application for Fertile Notify - a modern, responsive subscriber dashboard built with React and TypeScript for managing notification subscriptions and settings.

## Overview

This project contains the user-facing web interface for the Fertile Notify notification platform. It provides subscribers with a comprehensive dashboard to manage their notification settings, subscription plans, API keys, and communication channel preferences.

## Key Features

### Subscriber Dashboard

- **Profile Management**
  - Update company name and contact information
  - Manage email and phone number
  - View and update account details

- **Multi-Channel Notification Management**
  - Enable/disable any supported notification channel
  - Configure per-channel settings (API keys, tokens, webhook URLs)
  - Supported channels: Email, SMS, Console/In-App, WhatsApp, Telegram, Discord, Slack, MS Teams, Firebase Push, Web Push, Webhook
  - Real-time channel status updates

- **Subscription Information**
  - View current subscription plan (Free, Pro, Enterprise)
  - Monitor monthly notification limit
  - Track current month's usage
  - Check subscription expiry date

- **API Key Management**
  - Create new API keys with custom names
  - View existing API keys (with prefix display)
  - Delete API keys
  - Secure one-time key display

- **Security**
  - Password update functionality
  - JWT-based authentication with access and refresh tokens
  - OTP (one-time password) verification on login
  - Automatic token refresh on expiry
  - Secure API communication

### Notification Template Management

- **Templates Page**
  - Browse all available notification templates (Default and Custom)
  - Filter templates by channel and event type
  - Create and update custom templates per channel and event type
  - Test templates by sending a notification query

- **Visual Email Editor**
  - Drag-and-drop email template builder with GrapesJS
  - MJML-based responsive email design
  - Real-time preview of email templates
  - Pre-built email components and blocks
  - Export email templates as HTML/MJML

- **Advanced Email Editor**
  - Monaco Editor integration for code editing
  - Syntax highlighting for MJML and HTML
  - Real-time MJML to HTML compilation
  - Split-pane view with code and preview
  - Advanced code editing features (IntelliSense, error detection)

- **Channel Design Panel**
  - Dedicated text-based template editor for non-email channels (SMS, WhatsApp, Telegram, Discord, Slack, MS Teams, etc.)
  - Per-channel character limits and constraints
  - Event-type selection for template assignment
  - Title and message body editing with live character count

### Statistics & Analytics

- **Statistics Page**
  - View notification usage statistics by period (daily, weekly, monthly)
  - Breakdown of successful vs. failed notifications
  - Usage breakdown by channel and event type
  - Subscription usage summary (limit, used, remaining, expiry)

### User Experience

- **Home Page**: Modern landing page with feature highlights
- **Pricing Page**: Transparent subscription plan comparison (Free, Pro, Enterprise)
- **Registration**: New subscriber sign-up with subscription plan selection
- **Login**: Secure authentication with OTP verification and JWT tokens
- **Templates Management**: Browse, create, and test notification templates
- **Statistics Dashboard**: Usage analytics and subscription tracking
- **Email Template Editors**: Visual and advanced code-based email design tools
- **Channel Design Panel**: Text template editor for all non-email channels
- **Console Logs Viewer**: In-app modal for viewing notification delivery logs
- **Responsive Design**: Mobile-first design with Tailwind CSS
- **Modern UI**: Gradient backgrounds, animations, and polished components

## Tech Stack

### Core Technologies
- **React 19.2**: Modern React with latest features
- **TypeScript 5.9**: Type-safe development
- **Vite (Rolldown) 7.2**: Fast build tool with HMR (Hot Module Replacement)
- **React Router 7.13**: Client-side routing

### UI & Styling
- **Tailwind CSS 3.4**: Utility-first CSS framework
- **Custom CSS**: Gradient animations and modern effects
- **Responsive Design**: Mobile-first approach

### Email Design Tools
- **Monaco Editor 0.55**: Advanced code editor with IntelliSense
- **GrapesJS 0.22**: Visual page/email builder framework
- **GrapesJS MJML 1.0**: MJML plugin for GrapesJS
- **MJML Browser 4.18**: Client-side MJML to HTML compiler

### HTTP & API
- **Axios 1.13**: HTTP client for API communication
- **JWT Authentication**: Token-based secure authentication

### Development Tools
- **ESLint 9.39**: Code linting and quality
- **TypeScript ESLint 8.46**: TypeScript-specific linting rules
- **PostCSS 8.5**: CSS processing with Autoprefixer

## Project Structure

```
frontend/
├── src/
│   ├── api/                    # API service layer
│   │   ├── axiosClient.ts      # Configured Axios instance (with token refresh)
│   │   ├── authService.ts      # Authentication API calls (login, OTP, register)
│   │   ├── subscriberService.ts # Subscriber management API calls
│   │   ├── statisticService.ts # Statistics & analytics API calls
│   │   └── templateSevice.ts   # Template management API calls
│   │
│   ├── components/             # Reusable UI components
│   │   ├── ChannelSettingsModal.tsx # Per-channel configuration modal
│   │   └── ConsoleLogsModal.tsx     # In-app notification log viewer
│   │
│   ├── constants/              # Shared constants
│   │   ├── channels.ts         # Notification channel metadata and routes
│   │   └── eventTypes.ts       # Event type definitions (must match backend)
│   │
│   ├── pages/                  # Page components
│   │   ├── HomePage.tsx                     # Landing page
│   │   ├── LoginPage.tsx                    # Login page with OTP verification
│   │   ├── RegisterPage.tsx                 # Registration page
│   │   ├── DashboardPage.tsx                # Subscriber dashboard
│   │   ├── PricingPlanPage.tsx              # Subscription plan comparison
│   │   ├── TemplatesPage.tsx                # Notification template management
│   │   ├── StatisticsPage.tsx               # Usage statistics & analytics
│   │   ├── ChannelDesignPanelPage.tsx       # Text template editor for non-email channels
│   │   ├── EmailDesignVisualPanelPage.tsx   # Visual email editor (GrapesJS)
│   │   └── EmailDesignAdvancedPanelPage.tsx # Advanced email editor (Monaco)
│   │
│   ├── types/                  # TypeScript type definitions
│   │   ├── api.ts              # Generic API response wrapper types
│   │   ├── auth.ts             # Authentication types
│   │   ├── subscriber.ts       # Subscriber-related types
│   │   ├── statistic.ts        # Statistics and usage types
│   │   ├── template.ts         # Template and notification log types
│   │   └── mjml-browser.d.ts   # MJML browser type declarations
│   │
│   ├── assets/                 # Static assets
│   ├── App.tsx                 # Main application component with routing
│   ├── main.tsx                # Application entry point
│   ├── App.css                 # Application styles
│   └── index.css               # Global styles with Tailwind
│
├── public/                     # Public static files
├── index.html                  # HTML entry point
├── vite.config.ts              # Vite configuration
├── tsconfig.json               # TypeScript configuration
├── tailwind.config.js          # Tailwind CSS configuration
├── postcss.config.js           # PostCSS configuration
├── eslint.config.js            # ESLint configuration
├── package.json                # Dependencies and scripts
└── README.md                   # This file
```

## Getting Started

### Prerequisites

- [Node.js](https://nodejs.org/) version 18.x or higher
- [npm](https://www.npmjs.com/) version 9.x or higher (comes with Node.js)
- Running instance of Fertile Notify backend API

### Installation

1. **Navigate to the frontend directory**
   ```bash
   cd frontend
   ```

2. **Install dependencies**
   ```bash
   npm install
   ```

3. **Configure API endpoint**

   The frontend reads the API base URL from the `VITE_API_URL` environment variable. Create a `.env` file in the `frontend/` directory:
   ```env
   VITE_API_URL=http://localhost:8080/api
   ```
   Update the URL to match the host and port where your Fertile Notify backend API is running.

### Running the Application

#### Development Mode

Start the development server with hot module replacement:

```bash
npm run dev
```

The application will be available at `http://localhost:5173` (default Vite port).

#### Building for Production

Build the application for production:

```bash
npm run build
```

The optimized production build will be created in the `dist/` directory.

#### Preview Production Build

Preview the production build locally:

```bash
npm run preview
```

### Running Linter

Check code quality and style:

```bash
npm run lint
```

## API Integration

The frontend communicates with the Fertile Notify backend API through the following services:

### Authentication Service (`authService.ts`)
- `login(data)`: Initiate login; backend sends an OTP to the subscriber's email
- `verifyOtp(data)`: Verify OTP code and receive access + refresh tokens
- `register(data)`: Register new subscriber with a subscription plan

### Subscriber Service (`subscriberService.ts`)
- `getProfile()`: Retrieve authenticated subscriber's profile
- `setCompanyName(data)`: Update company name
- `setContactInfo(data)`: Update email and phone number
- `setChannel(data)`: Enable/disable a notification channel
- `setPassword(data)`: Update account password
- `setApikey(data)`: Create new API key
- `getApiKeys()`: List all API keys
- `deleteApiKey(key)`: Delete an API key
- `setChannelSetting(data)`: Save per-channel configuration (API tokens, webhook URLs, etc.)
- `getChannelSetting(channel)`: Retrieve per-channel configuration

### Statistics Service (`statisticService.ts`)
- `getStatistics(period)`: Retrieve usage statistics for a given period (e.g. `"monthly"`, `"weekly"`)

### Template Service (`templateSevice.ts`)
- `getAllTemplates()`: Retrieve all available templates (Default and Custom)
- `createOrUpdateTemplate(data)`: Create or update a custom notification template
- `queryTemplate(data)`: Test a template by submitting a notification query
- `getNotificationLogs()`: Retrieve notification delivery log entries

### Authentication Flow

1. User submits email and password on the `/login` page
2. Backend validates credentials and sends an OTP to the subscriber's email
3. User enters the OTP on the verification screen
4. Backend validates the OTP and returns an `accessToken` and a `refreshToken`
5. Tokens are stored in `localStorage` (`accessToken`, `refreshToken`)
6. The access token is automatically included in subsequent API requests via Axios interceptor
7. When the access token expires, the Axios interceptor automatically requests a new one using the refresh token
8. If the refresh also fails, the user is redirected to `/login`

## Available Scripts

| Command | Description |
|---------|-------------|
| `npm run dev` | Start development server with HMR |
| `npm run build` | Build for production (TypeScript + Vite) |
| `npm run preview` | Preview production build locally |
| `npm run lint` | Run ESLint to check code quality |

## Configuration Files

### Vite Configuration (`vite.config.ts`)
- React plugin configuration
- Build settings
- Development server options

### TypeScript Configuration
- `tsconfig.json`: Base TypeScript configuration
- `tsconfig.app.json`: Application-specific settings
- `tsconfig.node.json`: Node/build tool settings

### Tailwind Configuration (`tailwind.config.js`)
- Content paths for purging unused styles
- Custom theme extensions
- Plugin configurations

## Architecture Role

The frontend is the presentation layer in the Fertile Notify architecture:

**Depends on:**
- **Backend API** - FertileNotify.API for all data operations and authentication

**Responsibilities:**
- User interface and experience
- Client-side routing and navigation
- Form validation and user input handling
- State management for UI components
- API communication via HTTP requests
- Access token and refresh token management
- Responsive design and accessibility

**Does NOT contain:**
- Business logic (handled by backend Application layer)
- Data persistence (managed by backend Infrastructure layer)
- Authentication logic (delegated to backend API)

## Application Routes

| Route | Component | Description |
|-------|-----------|-------------|
| `/` | Redirect | Redirects to `/home` |
| `/home` | `HomePage` | Landing page with feature highlights |
| `/login` | `LoginPage` | Login with OTP verification |
| `/register` | `RegisterPage` | New subscriber sign-up |
| `/pricing` | `PricingPlanPage` | Subscription plan comparison |
| `/dashboard` | `DashboardPage` | Subscriber management dashboard |
| `/templates` | `TemplatesPage` | Notification template management |
| `/statistics` | `StatisticsPage` | Usage analytics and subscription tracking |
| `/channel-editor` | `ChannelDesignPanelPage` | Text template editor for non-email channels |
| `/email-visual-editor` | `EmailDesignVisualPanelPage` | Visual email editor (GrapesJS + MJML) |
| `/email-advanced-editor` | `EmailDesignAdvancedPanelPage` | Advanced email editor (Monaco Editor) |

## Development Guidelines

### Code Style
- Follow TypeScript best practices
- Use functional components with hooks
- Maintain consistent component structure
- Use Tailwind utility classes for styling
- Keep components focused and reusable

### Type Safety
- Define proper TypeScript interfaces for all data structures
- Avoid using `any` type
- Use type inference where appropriate
- Define API response types

### State Management
- Use React hooks (`useState`, `useEffect`, `useCallback`)
- Keep state close to where it's used
- Consider context API for global state if needed

## Browser Support

The application is built with modern web standards and supports:
- Chrome/Edge (latest 2 versions)
- Firefox (latest 2 versions)
- Safari (latest 2 versions)

## Future Enhancements

Planned features for future releases:
- Template versioning and history
- Real-time notification preview
- Dark/light theme toggle
- Notification scheduling UI
- Team collaboration features
- Multi-language support

## Contributing

This frontend is part of the Fertile Notify project. For contribution guidelines, please refer to the main repository README.

## License

This project is licensed under the **GNU General Public License v3.0** - see the main repository LICENSE file for details.

---

For more information about the Fertile Notify platform, see the [main repository](https://github.com/EnesEfeTokta/Fertile-Notify).
