import React from 'react';
import userEvent from '@testing-library/user-event';
import { render, fireEvent, screen } from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';
import Login from './../auth/login/login';
import { invalidLoginEntries, validLoginEntry } from './data/inputData';

let localStorageGetSpy = null;

beforeEach(() => {
	localStorageGetSpy = jest
		.spyOn(Storage.prototype, 'getItem')
		.mockReturnValue(
			'[{"email":"test@example.com","password":"12345678","firstName":"Test","lastName":"User"}]'
		);
});

afterEach(() => {
	localStorageGetSpy.mockRestore();
});

it('renders without crashing', () => {
	render(
		<MemoryRouter>
			<Login />
		</MemoryRouter>
	);
});

describe('Login input test', () => {
	it.each(invalidLoginEntries)(
		'check combination for validity',
		async (loginEntry) => {
			render(
				<MemoryRouter>
					<Login />
				</MemoryRouter>
			);

			const emailInput = screen.getByPlaceholderText(/example.com/i);
			userEvent.type(emailInput, loginEntry.email);
			const passwordInput = screen.getByPlaceholderText(/password/i);
			userEvent.type(passwordInput, loginEntry.password);

			const submitButton = screen.getByText(/submit/i);
			expect(submitButton).toBeEnabled();

			fireEvent.click(submitButton);
			expect(
				await screen.findByText(/Invalid email or password/i)
			).not.toBeNull();
			expect(localStorageGetSpy).toHaveBeenCalled();
		}
	);

	it('valid input success', async () => {
		render(
			<MemoryRouter>
				<Login />
			</MemoryRouter>
		);

		const emailInput = screen.getByPlaceholderText(/example.com/i);
		userEvent.type(emailInput, validLoginEntry.email);

		const submitButton = screen.getByText(/submit/i);
		expect(submitButton).not.toBeEnabled();

		const passwordInput = screen.getByPlaceholderText(/password/i);
		userEvent.type(passwordInput, validLoginEntry.password);

		expect(await screen.findByText(/submit/i)).toBeEnabled();

		fireEvent.click(submitButton);
		expect(screen.queryByText(/Invalid email or password/i)).toBeNull();
	});
});
