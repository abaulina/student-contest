import React from 'react';
import './notFound.css';
const NotFound = () => {
	return (
		<div className='d-flex align-items-center flex-column'>
			<p className='notFound'>
				Ooops! The page you're looking for has been removed, renamed or
				unavailable
				<span aria-label='confused face'>😕</span>
			</p>
			<a role='button' href='/' className='btn btn-outline-primary'>
				Take me back to the homepage
			</a>
		</div>
	);
};

export default NotFound;
