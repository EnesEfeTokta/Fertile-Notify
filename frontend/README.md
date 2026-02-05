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
  
- **Notification Channel Management**
  - Enable/disable Email notifications
  - Enable/disable SMS notifications
  - Enable/disable Console/In-App notifications
  - Real-time channel status updates

- **Subscription Information**
  - View current subscription plan (Free, Basic, Premium)
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
  - JWT-based authentication
  - Secure API communication

### User Experience

- **Home Page**: Modern landing page with feature highlights
- **Registration**: New subscriber sign-up with subscription plan selection
- **Login**: Secure authentication with JWT tokens
- **Responsive Design**: Mobile-first design with Tailwind CSS
- **Modern UI**: Gradient backgrounds, animations, and polished components

## Tech Stack

### Core Technologies
- **React 19.2**: Modern React with latest features
- **TypeScript 5.9**: Type-safe development
- **Vite (Rolldown)**: Fast build tool with HMR (Hot Module Replacement)
- **React Router 7.13**: Client-side routing

### UI & Styling
- **Tailwind CSS 3.4**: Utility-first CSS framework
- **Custom CSS**: Gradient animations and modern effects
- **Responsive Design**: Mobile-first approach

### HTTP & API
- **Axios 1.13**: HTTP client for API communication
- **JWT Authentication**: Token-based secure authentication

### Development Tools
- **ESLint**: Code linting and quality
- **TypeScript ESLint**: TypeScript-specific linting rules
- **PostCSS**: CSS processing with Autoprefixer

## Project Structure

```
frontend/
├── src/
│   ├── api/                    # API service layer
│   │   ├── axiosClient.ts      # Configured Axios instance
│   │   ├── authService.ts      # Authentication API calls
│   │   └── subscriberService.ts # Subscriber management API calls
│   │
│   ├── pages/                  # Page components
│   │   ├── HomePage.tsx        # Landing page
│   │   ├── LoginPage.tsx       # Login page
│   │   ├── RegisterPage.tsx    # Registration page
│   │   └── DashboardPage.tsx   # Subscriber dashboard
│   │
│   ├── types/                  # TypeScript type definitions
│   │   ├── auth.ts             # Authentication types
│   │   └── subscriber.ts       # Subscriber-related types
│   │
│   ├── assets/                 # Static assets
│   ├── App.tsx                 # Main application component
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
   
   Update the base URL in `src/api/axiosClient.ts` if your backend API is running on a different host/port:
   ```typescript
   const axiosClient = axios.create({
     baseURL: 'http://localhost:5080/api',  // Update this URL
     headers: {
       'Content-Type': 'application/json',
     }
   });
   ```

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
- `login(email, password)`: Authenticate subscriber and receive JWT token
- `register(data)`: Register new subscriber with subscription plan

### Subscriber Service (`subscriberService.ts`)
- `getProfile()`: Retrieve authenticated subscriber's profile
- `setCompanyName(data)`: Update company name
- `setContactInfo(data)`: Update email and phone number
- `setChannel(data)`: Enable/disable notification channels
- `setPassword(data)`: Update account password
- `setApikey(data)`: Create new API key
- `getApiKeys()`: List all API keys
- `deleteApiKey(key)`: Delete an API key

### Authentication Flow

1. User logs in via `/login` page
2. Backend returns JWT token
3. Token is stored in `localStorage`
4. Token is automatically included in subsequent API requests via Axios interceptor
5. Protected routes require valid token

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
- JWT token management
- Responsive design and accessibility

**Does NOT contain:**
- Business logic (handled by backend Application layer)
- Data persistence (managed by backend Infrastructure layer)
- Authentication logic (delegated to backend API)

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
- Real-time notification preview
- Advanced analytics dashboard
- Notification history and logs
- Template customization interface
- Multi-language support
- Dark/light theme toggle
- Notification scheduling UI

## Contributing

This frontend is part of the Fertile Notify project. For contribution guidelines, please refer to the main repository README.

## License

This project is licensed under the **GNU General Public License v3.0** - see the main repository LICENSE file for details.

---

For more information about the Fertile Notify platform, see the [main repository](https://github.com/EnesEfeTokta/Fertile-Notify).
