# Fertile Notify Frontend

The frontend web application for Fertile Notify - a modern, responsive subscriber dashboard built with React and TypeScript for managing notification subscriptions, templates, workflows, recipients, and settings.

## Overview

This project contains the user-facing web interface for the Fertile Notify notification platform. It provides subscribers with a comprehensive dashboard to manage their notification settings, subscription plans, API keys, communication channel preferences, notification workflows, recipient blacklists, and payment/billing.

## Key Features

### Account & Dashboard

- **Account Management** (`/account`)
  - Update company name, description, logo URL, website, and location
  - Manage email and phone number
  - Update account password
  - Export account data
  - Delete account

- **Subscriber Dashboard** (`/dashboard`)
  - High-level overview of the subscriber's account
  - Quick access to all platform features

- **Multi-Channel Notification Management**
  - Enable/disable any supported notification channel
  - Configure per-channel settings (API keys, tokens, webhook URLs)
  - Supported channels: Email, SMS, Console/In-App, WhatsApp, Telegram, Discord, Slack, MS Teams, Firebase Push, Web Push, Webhook
  - Real-time channel status updates

- **Subscription Information**
  - View current subscription plan (Free, Pro, Enterprise)
  - Monitor monthly notification limit and current usage
  - Check subscription expiry date

- **API Key Management** (`/api-keys`)
  - Create new API keys with custom names
  - View existing API keys (with prefix display)
  - Manage API key scopes and active/inactive status
  - Delete API keys
  - Secure one-time key display on creation

- **Security**
  - Password update functionality
  - JWT-based authentication with access and refresh tokens
  - OTP (one-time password) verification on login
  - Automatic token refresh on expiry
  - Secure API communication

### Notification Template Management

- **Templates Page** (`/templates`)
  - Browse all available notification templates (Default and Custom)
  - Filter templates by channel and event type
  - Create and update custom templates per channel and event type
  - Test templates by sending a notification query

- **Visual Email Editor** (`/email-visual-editor`)
  - Drag-and-drop email template builder with GrapesJS
  - MJML-based responsive email design
  - Real-time preview of email templates
  - Pre-built email components and blocks
  - Export email templates as HTML/MJML

- **Advanced Email Editor** (`/email-advanced-editor`)
  - Monaco Editor integration for code editing
  - Syntax highlighting for MJML and HTML
  - Real-time MJML to HTML compilation
  - Split-pane view with code and preview
  - Advanced code editing features (IntelliSense, error detection)

- **Per-Channel Design Panels**
  - Dedicated text-based template editor for each non-email channel
  - Individual routes for SMS, Console, WhatsApp, Telegram, Discord, Slack, MS Teams, Firebase Push, Web Push, and Webhook
  - Per-channel character limits and constraints
  - Event-type selection for template assignment
  - Title and message body editing with live character count

### Workflow Management

- **Workflows Page** (`/workflows`)
  - Create, view, update, and delete notification workflows
  - Assign event types and triggers to workflows
  - Configure workflow channels, subject, and body
  - Activate/deactivate workflows
  - Manually trigger workflows by event trigger name
  - Cron-based scheduling support

### Notification Logs

- **Logs Page** (`/logs`)
  - Browse notification delivery logs
  - Configurable log limit (default: 50)
  - Expandable log entries with full detail
  - Status indicators (delivered, failed, etc.)
  - Per-channel metadata display

### Blacklist Management

- **Blacklist Page** (`/blacklist`)
  - View all blacklisted recipient addresses
  - Add recipient addresses to the blacklist (per channel or all channels)
  - Edit existing blacklist entries
  - Activate/deactivate blacklist entries
  - Delete blacklist entries

### Recipients Manager

- **Recipients Manager** (`/recipients-manager`) — public-facing page
  - Allow recipients to unsubscribe from specific channels or all notifications
  - Submit spam/abuse complaints with reason and description

- **Unsubscribe Page** (`/unsubscribe`)
  - Token-based one-click unsubscribe flow for email recipients

### Statistics & Analytics

- **Statistics Page** (`/statistics`)
  - View notification usage statistics by period (daily, weekly, monthly)
  - Breakdown of successful vs. failed notifications
  - Usage breakdown by channel and event type
  - Subscription usage summary (limit, used, remaining, expiry)

### Payment & Billing

- **Buy Credits Page** (`/buy-credits`)
  - Purchase extra notification credits via Stripe
  - Secure payment flow with Stripe Elements

- **Payment History Page** (`/payment-history`)
  - View past payment transactions and credit purchases

### System Notifications

- **System Notifications Dropdown** (global, in app shell)
  - In-app system notification bell with unread count badge
  - Mark individual notifications as read
  - View all system notifications (read/unread/all)

### Info & Public Pages

- **Home Page** (`/home`): Modern landing page with feature highlights
- **Pricing Page** (`/pricing`): Transparent subscription plan comparison (Free, Pro, Enterprise)
- **About Us** (`/about`): Company and project information
- **Contact** (`/contact`): Contact form and information
- **Documentation** (`/documentation`): Platform usage documentation
- **Changelog** (`/changelog`): Release notes and version history
- **API Reference** (`/api-reference`): API endpoint reference for subscribers
- **Privacy Policy** (`/privacy`): Privacy policy page
- **Terms of Service** (`/terms`): Terms of service page

### User Experience

- **Registration**: New subscriber sign-up with subscription plan selection
- **Login**: Secure authentication with OTP verification and JWT tokens
- **App Shell**: Consistent sidebar navigation and layout across all authenticated pages
- **Toast Notifications**: Global toast notification system for user feedback
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
- **Stripe.js / React Stripe.js**: Stripe payment integration for credit purchases

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
│   │   ├── templateSevice.ts   # Template management API calls
│   │   ├── workflowService.ts  # Notification workflow API calls
│   │   ├── blacklistService.ts # Recipient blacklist API calls
│   │   ├── paymentService.ts   # Payment & billing API calls (Stripe)
│   │   ├── publicService.ts    # Public (unauthenticated) API calls (unsubscribe, complaints)
│   │   └── systemNotificationService.ts # System notification API calls
│   │
│   ├── components/             # Reusable UI components
│   │   ├── AppShell.tsx                 # Authenticated page layout with sidebar navigation
│   │   ├── ChannelSettingsModal.tsx     # Per-channel configuration modal
│   │   ├── ConsoleLogsModal.tsx         # In-app notification log viewer modal
│   │   ├── SystemNotificationsDropdown.tsx # System notification bell & dropdown
│   │   └── Toast.tsx                    # Global toast notification system
│   │
│   ├── constants/              # Shared constants
│   │   ├── channels.ts         # Notification channel metadata and routes
│   │   ├── eventTypes.ts       # Event type definitions (must match backend)
│   │   └── legal.ts            # Legal page content (privacy policy, terms of service)
│   │
│   ├── pages/                  # Page components
│   │   ├── info/                                # Informational / public pages
│   │   │   ├── AboutUsPage.tsx                  # About us page
│   │   │   ├── ApiReferencePage.tsx             # API reference documentation
│   │   │   ├── ChangelogPage.tsx                # Release notes and changelog
│   │   │   ├── ContactPage.tsx                  # Contact page
│   │   │   ├── DocumentationPage.tsx            # Platform documentation
│   │   │   └── InfoPageLayout.tsx               # Shared layout for info pages
│   │   │
│   │   ├── HomePage.tsx                         # Landing page
│   │   ├── LoginPage.tsx                        # Login page with OTP verification
│   │   ├── RegisterPage.tsx                     # Registration page
│   │   ├── DashboardPage.tsx                    # Subscriber dashboard overview
│   │   ├── AccountPage.tsx                      # Account & profile management
│   │   ├── ApiKeysPage.tsx                      # API key management
│   │   ├── PricingPlanPage.tsx                  # Subscription plan comparison
│   │   ├── BuyCreditsPage.tsx                   # Purchase extra notification credits
│   │   ├── PaymentHistoryPage.tsx               # Payment transaction history
│   │   ├── TemplatesPage.tsx                    # Notification template management
│   │   ├── StatisticsPage.tsx                   # Usage statistics & analytics
│   │   ├── LogsPage.tsx                         # Notification delivery logs
│   │   ├── WorkflowPage.tsx                     # Notification workflow management
│   │   ├── BlacklistPage.tsx                    # Recipient blacklist management
│   │   ├── RecipientsManagerPage.tsx            # Public unsubscribe & complaint page
│   │   ├── UnsubscribePage.tsx                  # Token-based one-click unsubscribe
│   │   ├── LegalPage.tsx                        # Privacy policy / terms of service
│   │   ├── EmailDesignVisualPanelPage.tsx       # Visual email editor (GrapesJS)
│   │   ├── EmailDesignAdvancedPanelPage.tsx     # Advanced email editor (Monaco)
│   │   ├── SmsDesignPanelPage.tsx               # SMS template editor
│   │   ├── ConsoleDesignPanelPage.tsx           # Console/In-App template editor
│   │   ├── WhatsappDesignPanelPage.tsx          # WhatsApp template editor
│   │   ├── TelegramDesignPanelPage.tsx          # Telegram template editor
│   │   ├── DiscordDesignPanelPage.tsx           # Discord template editor
│   │   ├── SlackDesignPanelPage.tsx             # Slack template editor
│   │   ├── MsteamsDesignPanelPage.tsx           # MS Teams template editor
│   │   ├── FirebasepushDesignPanelPage.tsx      # Firebase Push template editor
│   │   ├── WebpushDesignPanelPage.tsx           # Web Push template editor
│   │   └── WebhookDesignPanelPage.tsx           # Webhook template editor
│   │
│   ├── types/                  # TypeScript type definitions
│   │   ├── api.ts              # Generic API response wrapper types
│   │   ├── auth.ts             # Authentication types
│   │   ├── subscriber.ts       # Subscriber-related types
│   │   ├── statistic.ts        # Statistics and usage types
│   │   ├── template.ts         # Template and notification log types
│   │   ├── workflow.ts         # Workflow types
│   │   ├── blacklist.ts        # Blacklist entry types
│   │   ├── payment.ts          # Payment and billing types
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

   The frontend reads the API base URL and Stripe public key from environment variables. Create a `.env` file in the `frontend/` directory:
   ```env
   VITE_API_URL=http://localhost:8080/api
   VITE_STRIPE_PUBLISHABLE_KEY=pk_test_your_publishable_key
   ```
   - `VITE_API_URL`: URL of the running Fertile Notify backend API
   - `VITE_STRIPE_PUBLISHABLE_KEY`: Stripe publishable key for payment processing (required for the Buy Credits feature)

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
- `updateProfile(data)`: Update company info and contact details (name, description, logo, website, location, email, phone)
- `setChannel(data)`: Enable/disable a notification channel
- `setPassword(data)`: Update account password
- `setApikey(data)`: Create new API key
- `getApiKeys()`: List all API keys
- `deleteApiKey(key)`: Delete an API key
- `updateApiKeyScopes(key, data)`: Update the scopes of an API key
- `updateApiKeyStatus(key, data)`: Enable or disable an API key
- `setChannelSetting(data)`: Save per-channel configuration (API tokens, webhook URLs, etc.)
- `getChannelSetting(channel)`: Retrieve per-channel configuration
- `buyCredits(count)`: Add extra notification credits to the subscription
- `deleteAccount()`: Permanently delete the subscriber account
- `exportData()`: Export all subscriber data as a JSON file

### Statistics Service (`statisticService.ts`)
- `getStatistics(period)`: Retrieve usage statistics for a given period (e.g. `"monthly"`, `"weekly"`)

### Template Service (`templateSevice.ts`)
- `getAllTemplates()`: Retrieve all available templates (Default and Custom)
- `createOrUpdateTemplate(data)`: Create or update a custom notification template
- `queryTemplate(data)`: Test a template by submitting a notification query
- `getNotificationLogs(limit)`: Retrieve notification delivery log entries (default limit: 50)

### Workflow Service (`workflowService.ts`)
- `listWorkflows()`: List all notification workflows
- `getWorkflow(id)`: Get a specific workflow by ID
- `addWorkflow(req)`: Create a new notification workflow
- `updateWorkflow(req)`: Update an existing workflow
- `deleteWorkflow(id)`: Delete a workflow
- `activateWorkflow(id)`: Activate a workflow
- `deactivateWorkflow(id)`: Deactivate a workflow
- `triggerWorkflow(eventTrigger)`: Manually trigger a workflow by event trigger name

### Blacklist Service (`blacklistService.ts`)
- `getAll()`: Retrieve all blacklisted recipient entries
- `add(request)`: Add a recipient address to the blacklist
- `update(id, request)`: Update a blacklist entry
- `delete(id)`: Remove a recipient from the blacklist

### Payment Service (`paymentService.ts`)
- `createExtraCreditPaymentIntent(data)`: Create a Stripe payment intent for purchasing extra credits
- `getPaymentHistory()`: Retrieve payment transaction history

### Public Service (`publicService.ts`) — unauthenticated
- `unsubscribe(data)`: Allow a recipient to unsubscribe from notifications (token-based)
- `submitComplaint(data)`: Submit a spam/abuse complaint about notifications

### System Notification Service (`systemNotificationService.ts`)
- `getSystemNotifications(status)`: Retrieve system notifications filtered by status (`"all"`, `"read"`, `"unread"`)
- `markSystemNotificationAsRead(notificationId)`: Mark a system notification as read

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
| `/dashboard` | `DashboardPage` | Subscriber account overview |
| `/account` | `AccountPage` | Profile, password, and account settings |
| `/api-keys` | `ApiKeysPage` | API key management |
| `/buy-credits` | `BuyCreditsPage` | Purchase extra notification credits |
| `/payment-history` | `PaymentHistoryPage` | Payment transaction history |
| `/templates` | `TemplatesPage` | Notification template management |
| `/statistics` | `StatisticsPage` | Usage analytics and subscription tracking |
| `/logs` | `LogsPage` | Notification delivery logs |
| `/workflows` | `WorkflowPage` | Notification workflow management |
| `/blacklist` | `BlacklistPage` | Recipient blacklist management |
| `/recipients-manager` | `RecipientsManagerPage` | Public unsubscribe and complaint form |
| `/unsubscribe` | `UnsubscribePage` | Token-based one-click unsubscribe |
| `/email-visual-editor` | `EmailDesignVisualPanelPage` | Visual email editor (GrapesJS + MJML) |
| `/email-advanced-editor` | `EmailDesignAdvancedPanelPage` | Advanced email editor (Monaco Editor) |
| `/sms-editor` | `SmsDesignPanelPage` | SMS template editor |
| `/console-editor` | `ConsoleDesignPanelPage` | Console/In-App template editor |
| `/whatsapp-editor` | `WhatsappDesignPanelPage` | WhatsApp template editor |
| `/telegram-editor` | `TelegramDesignPanelPage` | Telegram template editor |
| `/discord-editor` | `DiscordDesignPanelPage` | Discord template editor |
| `/slack-editor` | `SlackDesignPanelPage` | Slack template editor |
| `/msteams-editor` | `MsteamsDesignPanelPage` | MS Teams template editor |
| `/firebasepush-editor` | `FirebasepushDesignPanelPage` | Firebase Push template editor |
| `/webpush-editor` | `WebpushDesignPanelPage` | Web Push template editor |
| `/webhook-editor` | `WebhookDesignPanelPage` | Webhook template editor |
| `/about` | `AboutUsPage` | About the project and team |
| `/contact` | `ContactPage` | Contact information and form |
| `/documentation` | `DocumentationPage` | Platform usage documentation |
| `/changelog` | `ChangelogPage` | Release notes and version history |
| `/api-reference` | `ApiReferencePage` | API reference for subscribers |
| `/privacy` | `LegalPage (privacy)` | Privacy policy |
| `/terms` | `LegalPage (terms)` | Terms of service |

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
- Multi-language support
- Advanced workflow conditions and branching
- Team collaboration features

## Contributing

This frontend is part of the Fertile Notify project. For contribution guidelines, please refer to the main repository README.

## License

This project is licensed under the **GNU General Public License v3.0** - see the main repository LICENSE file for details.

---

For more information about the Fertile Notify platform, see the [main repository](https://github.com/EnesEfeTokta/Fertile-Notify).
