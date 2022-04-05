import React from 'react';
import { render } from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';
import Main from '../main/mainPage';

jest.mock('../auth/useAuth', () => {
	const originalModule = jest.requireActual('../auth/useAuth');
	return {
		__esModule: true,
		...originalModule,
		default: () => ({
			isAuthenticated: true
		})
	};
});

describe('Main when isAuthenticated', () => {
	it('renders without crashing', () => {
		render(
			<MemoryRouter>
				<Main />
			</MemoryRouter>
		);
	});
});

jest.mock('../auth/useAuth', () => {
	const originalModule = jest.requireActual('../auth/useAuth');
	return {
		__esModule: true,
		...originalModule,
		default: () => ({
			isAuthenticated: false
		})
	};
});

describe('Main when !isAuthenticated', () => {
	it('renders without crashing', () => {
		render(
			<MemoryRouter>
				<Main />
			</MemoryRouter>
		);
	});

	it('displays image and login form', () => {
		render(
			<MemoryRouter>
				<Main />
			</MemoryRouter>
		);

		const image = document.querySelector('[data-testid=img]');
		const form = document.querySelector('[data-testid=loginForm]');
		expect(image).toBeInTheDocument();
		expect(form).toBeInTheDocument();
	});
});
