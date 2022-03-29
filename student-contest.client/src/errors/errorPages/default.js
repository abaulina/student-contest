import React from 'react';
import { Link } from 'react-router-dom';
import './notFound.css';

const DefaultErrorPage = () => {
	return (
		<div className='d-flex not-found'>
			<p className='not-found'>Something is wrong. Please try again</p>
			<button className='button default'>
				<Link className='not-found' to={'/'}>
					Take me back to the homepage
				</Link>
			</button>
		</div>
	);
};

export default DefaultErrorPage;
