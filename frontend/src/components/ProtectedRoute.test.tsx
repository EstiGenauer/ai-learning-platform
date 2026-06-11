import { render, screen } from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';
import ProtectedRoute from './ProtectedRoute';
import { AuthProvider } from '../context/AuthContext';

const renderWithAuth = (ui: React.ReactElement, route = '/dashboard') =>
  render(
    <AuthProvider>
      <MemoryRouter initialEntries={[route]}>{ui}</MemoryRouter>
    </AuthProvider>
  );

test('redirects unauthenticated users to login', () => {
  renderWithAuth(
    <ProtectedRoute>
      <div>Secret content</div>
    </ProtectedRoute>
  );
  expect(screen.queryByText(/secret content/i)).not.toBeInTheDocument();
});
