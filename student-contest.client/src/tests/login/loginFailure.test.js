import React from 'react';
import userEvent from '@testing-library/user-event';
import { render, fireEvent, screen } from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';
import Login from '../../auth/login/login';
import { invalidLoginEntry } from '../data/inputData';

it('renders without crashing', () => {
	render(
		<MemoryRouter>
			<Login />
		</MemoryRouter>
	);
});

jest.mock('../../auth/useAuth', () => {
	const originalModule = jest.requireActual('../../auth/useAuth');
	return {
		__esModule: true,
		...originalModule,
		default: () => ({
			isAuthenticated: false,
			login: jest.fn,
			logout: jest.fn
		})
	};
});

describe('Login invalid input test', () => {
	it('invalid input error', async () => {
		render(
			<MemoryRouter>
				<Login />
			</MemoryRouter>
		);

		const emailInput = screen.getByPlaceholderText(/example.com/i);
		userEvent.type(emailInput, invalidLoginEntry.email);
		const passwordInput = screen.getByPlaceholderText(/password/i);
		userEvent.type(passwordInput, invalidLoginEntry.password);

		const submitButton = screen.getByText(/submit/i);
		expect(submitButton).toBeEnabled();

		fireEvent.click(submitButton);
		expect(
			await screen.findByText(/Invalid email or password/i)
		).not.toBeNull();
	});
});
