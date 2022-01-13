import React from 'react';
import '@testing-library/jest-dom';
import userEvent from '@testing-library/user-event';
import { render, act, fireEvent, screen } from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';
import { createMemoryHistory } from 'history';
import Login from './../auth/login/login';
import { invalidLoginEntries, validLoginEntry } from './inputData';

let localStorageGetSpy = null;
let history = null;

beforeEach(() => {
	localStorageGetSpy = jest
		.spyOn(Storage.prototype, 'getItem')
		.mockReturnValue(
			'[{"email":"test@example.com","password":"12345678","firstName":"Test","lastName":"User"}]'
		);
	history = createMemoryHistory();
});

afterEach(() => {
	localStorageGetSpy.mockRestore();
	history = null;
});

it('renders without crashing', () => {
	act(() => {
		render(
			<MemoryRouter history={history}>
				<Login />
			</MemoryRouter>
		);
	});
});

describe('Login input test', () => {
	test.each(invalidLoginEntries)(
		'check combination for validity',
		async (loginEntry) => {
			act(() => {
				render(
					<MemoryRouter history={history}>
						<Login />
					</MemoryRouter>
				);
			});

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
		act(() => {
			render(
				<MemoryRouter history={history}>
					<Login />
				</MemoryRouter>
			);
		});

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
