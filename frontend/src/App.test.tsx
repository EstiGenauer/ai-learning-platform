import { render, screen } from '@testing-library/react';
import App from './App';

test('renders login page on /login route', () => {
  window.history.pushState({}, 'Login', '/login');
  render(<App />);
  expect(screen.getByText(/welcome back/i)).toBeInTheDocument();
  expect(screen.getByPlaceholderText(/email/i)).toBeInTheDocument();
});
