import React from 'react';
import '@testing-library/jest-dom';
import { unmountComponentAtNode } from 'react-dom';
import { render, act, fireEvent, screen } from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';
import { createMemoryHistory } from 'history';
import Login from './../auth/login/login';

let container = null;
let localStorageGetSpy = null;
let history = null;

beforeEach(() => {
	container = document.createElement('div');
	document.body.appendChild(container);
	localStorageGetSpy = jest
		.spyOn(Storage.prototype, 'getItem')
		.mockReturnValue(
			'[{"email":"test@example.com","password":"12345678","firstName":"Test","lastName":"User"}]'
		);
	history = createMemoryHistory();
});

afterEach(() => {
	unmountComponentAtNode(container);
	container.remove();
	container = null;
	localStorageGetSpy.mockRestore();
	history = null;
});

it('renders without crashing', () => {
	act(() => {
		render(
			<MemoryRouter history={history}>
				<Login />, container
			</MemoryRouter>
		);
	});
});

const invalidLoginEntries = [
	{ email: 'abc.test.com', password: '12345678' },
	{ email: 'example@abc', password: 'pass$12D' },
	{ email: 'jane@yahoo.com', password: 'passW34E' },
	{ email: 'john_done@testing.com', password: 'Pa$$w0rd' }
];

describe('Login input test', () => {
	test.each(invalidLoginEntries)(
		'check combination for validity',
		async (loginEntry) => {
			act(() => {
				render(
					<MemoryRouter history={history}>
						<Login />, container
					</MemoryRouter>
				);
			});

			const emailInput = screen.queryByPlaceholderText(/example.com/i);
			fireEvent.change(emailInput, { target: { value: loginEntry.email } });
			const passwordInput = screen.queryByPlaceholderText(/password/i);
			fireEvent.change(passwordInput, {
				target: { value: loginEntry.password }
			});

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
		const validEntry = { email: 'test@example.com', password: '12345678' };
		act(() => {
			render(
				<MemoryRouter history={history}>
					<Login />, container
				</MemoryRouter>
			);
		});

		const emailInput = screen.queryByPlaceholderText(/example.com/i);
		fireEvent.change(emailInput, { target: { value: validEntry.email } });

		const submitButton = screen.getByText(/submit/i);
		expect(submitButton).not.toBeEnabled();

		const passwordInput = screen.queryByPlaceholderText(/password/i);
		fireEvent.change(passwordInput, {
			target: { value: validEntry.password }
		});

		expect(await screen.findByText(/submit/i)).toBeEnabled();

		fireEvent.click(submitButton);
		expect(screen.queryByText(/Invalid email or password/i)).toBeNull();
	});
});
